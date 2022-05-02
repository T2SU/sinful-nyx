using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class TemporaryState
    {
        private readonly Dictionary<string, TemporaryStateEntry> _states
            = new Dictionary<string, TemporaryStateEntry>();

        private PlayerState _state;

        public void Update(Player player)
        {
        }
    }
}
