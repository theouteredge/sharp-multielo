using NumSharp;
using NumSharp.Generic;

namespace theouteredge.mulielo
{
    /// <summary>
    /// Generalized Elo for multiplayer matchups (also simplifies to standard Elo for 1-vs-1 matchups).
    /// Does not allow ties.
    /// T Represents your "player" class or player identifier
    /// </summary>
    public class MultiElo<T>
    {
        private readonly float kValue;
        private readonly float dValue;
        private readonly float scoreFunctionBase;
        private readonly float logBase;
        Func<int, IEnumerable<double>> scoringFunc;

        public MultiElo(
            float kValue = 32,
            float dValue = 400,
            float scoreFunctionBase = 1,
            Func<int, IEnumerable<double>>? customScoring = null,
            float logBase = 10)
        {
            this.kValue = kValue;
            this.dValue = dValue;
            this.scoreFunctionBase = scoreFunctionBase;
            this.logBase = logBase;
            scoringFunc = customScoring != null
              ? customScoring
              : Scoring.Create(scoreFunctionBase);
        }

        /// <summary>
        /// Update ratings based on results. Takes an array of ratings before the matchup and returns an array with
        /// the updated ratings.Provided array should be ordered by the actual results(first place finisher's
        /// initial rating first, second place next, and so on).
        /// Example usage:
        /// >>> elo = MultiElo()
        /// >>> elo.get_new_ratings([1200, 1000])
        /// array([1207.68809835,  992.31190165])
        /// >>> elo.get_new_ratings([1200, 1000, 1100, 900])
        /// array([1212.01868209, 1012.15595083, 1087.84404917,  887.98131791])
        /// </summary>
        /// <param name="ratings">array of ratings(float values) in order of actual results</param>
        /// <param name="order">list where each value indicates the place the player in the same index of 
        /// initial_ratings finished in. Lower is better.Identify ties by entering the same value for players
        /// that tied. For example, [1, 2, 3] indicates that the first listed player won, the second listed player
        /// finished 2nd, and the third listed player finished 3rd. [1, 2, 2] would indicate that the second
        /// and third players tied for 2nd place. (default = range(len(initial_ratings))</param>
        /// <returns>array of updated ratings(float values) in same order as input</returns>
        public IEnumerable<double> CalculateRatings(
            IEnumerable<double> ratings,
            IEnumerable<int>? order = null)
        {
            var n = ratings.Count();
            var actualScores = CalculateActualScores(n, order);
            var expectedScores = CalculateExpectedScores(ratings.ToList());

            var scaleFactor = kValue * (n - 1);

            var adjustments = actualScores
                .Zip(expectedScores)
                .Select(x => scaleFactor * (x.First - x.Second));

            return ratings
                .Zip(adjustments)
                .Select(x => x.First + x.Second);

            /*
            return initial_ratings + scale_factor * (actual_scores - expected_scores)
            */
        }

        /// <summary>
        /// Return the scores to be awarded to the players based on the results.
        /// </summary>
        /// <param name="n">number of players in the matchup</param>
        /// <param name="positions">list indicating order of finish (see docstring for 
        /// MultiElo.get_new_ratings for more details</param>
        /// <returns>array of length n of scores to be assigned to first place, second place, and so on</returns>
        public IEnumerable<double> CalculateActualScores(int n, IEnumerable<int>? positions)
        {
            var order = positions == null 
                ? Enumerable.Range(1, n)
                : positions;

            var scores = scoringFunc(n);
            //scores = scores[np.argsort(np.argsort(result_order))]

            //algo to implement
            // assign all positions the correct score
            // anyone who tied should have there scores combined / number of tied people and the shared out
            // i.e. positions [2,2,1,3] for scores [0.5, 0.333333, 0.166667, 0.0] would be [0.25, 0.25, 0.5, 0.0] 

            var positionsAndScores = order
                .OrderBy(x => x)
                .Zip(scores)
                .GroupBy(
                    x => x.First,
                    x => x.Second,
                    (pos, scores) => Tuple.Create(pos, scores.Sum() / scores.Count())
                );

            return order.Join(
                    positionsAndScores,
                    x => x,
                    x => x.Item1,
                    (a,b) => Tuple.Create(a, b.Item2)
                )
                .Select(x => x.Item2);
        }



        /// <summary>
        /// Get the expected scores for all players given their ratings before the matchup
        /// </summary>
        /// <param name="ratings">A list of rating for all players pre game</param>
        /// <returns>A list of expected scores for all players</returns>
        public IEnumerable<double> CalculateExpectedScores(IList<double> ratings)
        {
            // create a matrix of player rating compared to all other players
            var diffMx = ratings.Select(player => 
                ratings.Select(opponent => opponent - player).ToArray()
            );

            // get individual contributions to expected score using logistic function
            var logMx = diffMx.Select(x =>
                x.Select(y => 1 / (1 + Math.Pow(logBase, y / dValue))).ToArray()
            ).ToList();
            
            var logisticMx = diagonalFill(logMx, _ => 0);
            var expectedScores = logisticMx.Select(x => x.Sum());

            var n = expectedScores.Count();
            var denom = n * (n - 1) / 2;

            var finalScores = expectedScores.Select(x => x / denom);
            if (Util.Within(finalScores.Sum(), 1, 0.000000000000001))
                return finalScores;
            else
                throw new Exception($"The ecpected scored should have added up to 1, but we got {finalScores.Sum()} for scores {Util.format(finalScores)}");
        }

        private List<double[]> diagonalFill(
            List<double[]> matrix, 
            Func<double, double> func)
        {
            var i = 0;
            return matrix.Select(x => {
                var items = x.ToArray();
                items[i] = func(items[i]);

                i++;
                return items;
            }).ToList();
        }

        /*
        def get_expected_scores(self, ratings: Union[List[float], np.ndarray]) -> np.ndarray:
            :param ratings: array of ratings for each player in a matchup
            :return: array of expected scores for all players
            """
            logger.debug(f"computing expected scores for {ratings}")
            if not isinstance(ratings, np.ndarray):
                ratings = np.array(ratings)
            if ratings.ndim > 1:
                raise ValueError(f"ratings should be 1-dimensional array (received {ratings.ndim})")

            # get all pairwise differences
            diff_mx = ratings - ratings[:, np.newaxis]
            logger.debug(f"diff_mx = \n{diff_mx}")

            # get individual contributions to expected score using logistic function
            logistic_mx = 1 / (1 + self._log_base** (diff_mx / self.d))
            np.fill_diagonal(logistic_mx, 0)
            logger.debug(f"logistic_mx = \n{logistic_mx}")

            # get each expected score (sum individual contributions, then scale)
            expected_scores = logistic_mx.sum(axis=1)
            n = len(ratings)
            denom = n* (n - 1) / 2  # number of individual head-to-head matchups between n players
            expected_scores = expected_scores / denom

            # this should be guaranteed, but check to make sure
            if not np.allclose(1, sum(expected_scores)):
                raise ValueError("expected scores do not sum to 1")
            logger.debug(f"calculated expected scores: {expected_scores}")
            return expected_scores
        */
    }
}