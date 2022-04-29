using System;
using UnityEngine;

namespace Sevens.Effects
{
    [Serializable]
    public class ActionEffectOption
    {
        public CameraShakeOption Shake;
        public AudioClip Sound;
        public GameObject Particle;

        public void Apply(Transform source, Vector3 pos, Vector2 offset)
        {
            if (Particle != null)
            {
                pos += (Vector3)offset;
                GameObject.Instantiate(Particle, pos, Quaternion.identity);
            }
        }
    }
}
