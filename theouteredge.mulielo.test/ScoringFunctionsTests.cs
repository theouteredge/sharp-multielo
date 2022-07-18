namespace theouteredge.mulielo.test
{
    public class ScoringFunctionsTests
    {
        Func<IEnumerable<double>, string> format = (scores) => $"[{string.Join(',', scores)}]";

        Func<IEnumerable<double>, List<double>> difference = (scores) =>
        {
            var result = new List<double>();
            var list = scores.ToList();
            for (var i = 0; i < list.Count -1; i++)
            {
                result.Add(Math.Abs(list[i] - list[i+1]));
            }

            return result;
        };


        [SetUp]
        public void Setup()
        {
        
        }

        [Test]
        public void Assert_That_Liner_Scoring_Works()
        {
            Enumerable.Range(3, 11).Select(n =>
            {
                var scoringMethod = Scoring.Create(1);
                var scores = scoringMethod(n);

                Assert.That(scores.Sum(), Is.EqualTo(1).Within(0.000000000000001), 
                    () => $"liner scoring algorithm does not sum to 1 for n={n}: {format(scores)}");
                
                Assert.That(scores.Min(), Is.EqualTo(0), 
                    () => $"linear score function does not have minimum score of 0 for n={n}: {format(scores)}");

                var diff = difference(scores);
                for (var i = 0; i < diff.Count() -1; i++)
                    Assert.That(diff[i] - diff[i+1], Is.EqualTo(0).Within(0.000000000000001), 
                        () => $"linear score function is not monotonically decreasing for n={n}: out by {diff[i] - diff[i+1]} {format(diff)}");

                return n;
            }).ToList();

            Assert.Pass();
        }
    }
}