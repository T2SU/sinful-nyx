// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sevens.Skills
{
    public class Cooltime
    {
        private readonly Dictionary<string, float> _dictionary
            = new Dictionary<string, float>();

        public bool IsBeing(string key, float cooltime)
        {
            if (!_dictionary.TryGetValue(key, out var time))
                return false;
            return Time.time < time + cooltime;
        }

        public void Set(string key)
        {
            _dictionary[key] = Time.time;
        }

        public void Reset(string key)
        {
            _dictionary.Remove(key);
        }

        public override string ToString()
        {
            var now = Time.time;
            return string.Join(", ", _dictionary.Select(kv => $"[{kv.Key}-{kv.Value:F02}s]"));
        }
    }
}
