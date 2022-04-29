// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using System;
using System.Collections;
using UnityEngine;

namespace Sevens.Utils
{
    public static class CoroutineUtility
    {
        public static IEnumerator WaitAndAction(float time, Action action)
        {
            if (time > 0)
                yield return new WaitForSeconds(time);
            action();
            yield break;
        }

        public static IEnumerator SlowTime(float timeScale, float time)
        {
            Time.timeScale = timeScale;
            float pauseEndTime = Time.realtimeSinceStartup + time;
            while (Time.realtimeSinceStartup < pauseEndTime)
                yield return 0;
            Time.timeScale = 1.0f;
        }
    }
}
