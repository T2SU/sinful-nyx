using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sevens.Utils
{
    public static class Singleton<T>
    {
        public static T Data { get; set; }
    }
}
