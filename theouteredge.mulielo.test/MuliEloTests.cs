using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theouteredge.mulielo.test
{
    public class MuliEloTests
    {
        //32, 400, 1, np.array([1000, 1000]), [0.5, 0.5], [1016, 984]
        //32, 400, 1.5, [1200, 1000], [0.75974693, 0.24025307], [1207.68809835, 992.31190165]),

        [Test]
        public void Assert_That_MultiElo_Works_As_Expected_For_Linier_Scoring()
        {
            var elo = new MultiElo<int>(32, 400, 1);
            var ratings = elo.CalculateRatings(new[] { 1000.0, 1000.0 }, new[] { 1, 2 });

            Assert.That(ratings, Is.EqualTo(new[] { 1016, 984 }));
        }

        [Test]
        public void Assert_That_MultiElo_Works_As_Expected_Exponential_Scoring()
        {
            var elo = new MultiElo<int>(32, 400, 1.5f);
            var ratings = elo.CalculateRatings(new[] { 1200.0, 1000.0 }, new[] { 1, 2 });

            Assert.That(ratings, Is.EqualTo(new[] { 1207.6880983472654, 992.3119016527346 }));
        }


        [Test]
        public void Assert_That_MultiElo_Works_As_Expected_3players_Exponential_Scoring()
        {
            var elo = new MultiElo<int>(32, 400, 2);
            var ratings = elo.CalculateRatings(new[] { 1200.0, 1000.0, 900.0 }, new[] { 1, 2, 3 });

            Assert.That(ratings, Is.EqualTo(new[] {
                1213.6796294520184,
                997.2198811060282,
                889.1004894419533 
            }));
        }

        [Test]
        public void Assert_That_MultiElo_Works_As_Expected_4players_Exponential_Scoring()
        {
            var elo = new MultiElo<int>(32, 400, 1.25f);
            var ratings = elo.CalculateRatings(new[] { 1200.0, 1000.0, 900.0, 1050.0 }, new[] { 1, 2, 3, 4 });

            Assert.That(ratings, Is.EqualTo(new[] {
                1214.8285708830222,
                1009.6423915045154,
                900.67244749099655,
                1024.8565901214658 }));
        }

        [Test]
        public void Assert_That_MultiElo_Works_As_Expected_22players_Exponential_Scoring()
        {
            var elo = new MultiElo<int>(32, 400, 1.25f);
            var ratings = elo.CalculateRatings(
                new[] { 6000.0, 5000.0, 2200.0, 1100.0, 900.0, 2478.0, 1200.0, 1000.0, 600.0, 3400.0, 1800.0, 1600.0, 2600.0, 
                1500.0, 1400.0, 3224.0, 1200.0, 1800.0, 2600.0, 900.0, 850.0, 950.0 }).ToArray();

            Assert.Pass();
        }
    }
}
