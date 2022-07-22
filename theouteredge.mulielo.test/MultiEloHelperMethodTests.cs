using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theouteredge.mulielo.test
{
    public class MultiEloHelperMethodTests
    {
        [Test]
        public void Assert_That_CalulcateExpectedScores_Works_For_3players()
        {
            var elo = new MultiElo<int>();
            var ratings = new List<double>() { 1200, 1000, 900 };
            var expected = elo.CalculateExpectedScores(ratings);

            Assert.Pass();
        }

        [Test]
        public void Assert_That_CalulateActualScores_Works_For_Std_Players()
        {
            var elo = new MultiElo<int>();
            var scores = elo.CalculateActualScores(4, new[] { 2, 4, 1, 3 });

            Assert.That(scores, Is.EqualTo(new[] { 0.33333333333333331, 0.0, 0.5, 0.16666666666666666 }));
        }

        [Test]
        public void Assert_That_CalulateActualScores_Works_For_Tied_Players()
        {
            var elo = new MultiElo<int>();
            var scores = elo.CalculateActualScores(4, new[] { 2, 2, 1, 3 });

            Assert.That(scores, Is.EqualTo(new[] { 0.25, 0.25, 0.5, 0 }));
        }

        [Test]
        public void Assert_That_CalulateActualScores_Works_With_Two_Tied_Players()
        {
            var elo = new MultiElo<int>();
            var scores = elo.CalculateActualScores(4, new[] { 2, 1, 2, 1 });

            Assert.That(scores, Is.EqualTo(new[] { 
                0.083333333333333329, 
                0.41666666666666663, 
                0.083333333333333329, 
                0.41666666666666663
            }));
        }
    }
}
