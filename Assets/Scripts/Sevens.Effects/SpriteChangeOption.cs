using System;
using UnityEngine;

namespace Sevens.Effects
{
    [Serializable]
    public class SpriteChangeOption
    {
        public SpriteRenderer Target;
        public Sprite ToChange;

        public SpriteChangeOption(SpriteRenderer target, Sprite toChange)
        {
            Target = target;
            ToChange = toChange;
        }
    }
}
