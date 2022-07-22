using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theouteredge.mulielo
{
    public static class Util
    {
        public static Func<IEnumerable<double>, string> format = 
            (scores) => $"[{string.Join(',', scores)}]";

        public static bool Within(double value, double expected, double tollerance)
        {
            if (value == expected)
                return true;

            var min = value - tollerance;
            var max = value + tollerance;
            return expected >= min || expected <= max;
        }
    }
}
