using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theouteredge.mulielo
{
    public static class Scoring
    {
        /// <summary>
        /// For a base value of 1 an Liner scoring method will be created:
        /// With the linear score function the "points" awarded scale linearly from first place
        /// through last place.For example, improving from 2nd to 1st place has the same sized
        /// benefit as improving from 5th to 4th place.
        /// 
        /// For base values > 1 and Exponetial function will be returned:
        /// With an exponential score function more points are awarded to the top
        /// finishers and the point distribution is flatter at the bottom.For example, improving
        /// from 2nd to 1st place is more valuable than improving from 5th place to 4th place. A
        /// larger base value means the scores will be more weighted towards the top finishers.
        /// </summary>
        /// <param name="_base">_base for the exponential score function (> 1) a value of 1 returns an liner scoring method</param>
        /// <returns>
        /// A function that takes parameter n for number of players and returns an array 
        /// of the points to assign to each place(summing to 1)
        /// </returns>
        public static Func<int, IEnumerable<double>> Create(double _base)
        {
            if (_base < 1)
                throw new ArgumentException($"The value of {_base} must be 1 or greater");

            if (_base == 1)
                return (n) => Liner(n);

            return (n) => Exponential(n, _base);
        }

        
        public static IEnumerable<double> Exponential(int n, double _base)
        {
            var output = Enumerable.Range(1, n).Select(p => (double)Math.Pow(_base, (n - (double)p) - 1));
            var sum = output.Sum();

            return output; // / sum;
            //TODO: workout what numpy does with arrays when you / it
        }

        private static IEnumerable<double> Liner(int n) =>
            Enumerable.Range(1, n).Select(p => (n - (double)p) / (n * (n - 1) / 2));
    }
}
