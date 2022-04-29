// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sevens.Utils
{
    public static class Randomizer
    {
        public static List<T> Shuffle<T>(List<T> input)
        {
            return input.OrderBy(d => Next()).ToList();
        }

        public static IOrderedEnumerable<T> Shuffle<T>(IEnumerable<T> input)
        {
            return input.OrderBy(d => Next());
        }

        public static T PickOneRand<T>(IEnumerable<T> input)
        {
            var t = input.ToList();
            if (t.Count == 0)
                return default;
            return t[Next(t.Count)];
        }

        private static int Next(int c)
        {
            return Random.Range(0, c);
        }

        private static int Next()
        {
            return Random.Range(int.MinValue, int.MaxValue);
        }
    }
}
