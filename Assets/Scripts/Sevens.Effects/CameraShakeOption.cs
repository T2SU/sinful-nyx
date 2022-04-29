using System;
using UnityEngine;

namespace Sevens.Effects
{
    [Serializable]
    public struct CameraShakeOption
    {
        public float Amplitude;
        public float Frequency;
        public float Time;

        public bool HasValue()
        {
            if (Mathf.Approximately(Time, 0.0f))
                return false;
            if (Mathf.Approximately(Amplitude, 0.0f))
                return false;
            if (Mathf.Approximately(Frequency, 0.0f))
                return false;
            return true;
        }
    }
}
