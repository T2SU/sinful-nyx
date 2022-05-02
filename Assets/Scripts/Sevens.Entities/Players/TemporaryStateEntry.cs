using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sevens.Entities.Players
{
    [Serializable]
    public abstract class TemporaryStateEntry : ScriptableObject
    {
        public float Value;
        public float End;
        public string Name;

        public abstract void OnApply(Player player);
        public abstract void OnCancel(Player player);
    }
}
