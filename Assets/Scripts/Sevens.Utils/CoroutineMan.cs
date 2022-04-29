// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sevens.Utils
{
    [Flags]
    public enum CoroutineManType
    {
        None = 0,
        Tweener = 1 << 0,
        Coroutine = 1 << 1,

        All = Tweener | Coroutine
    }

    public class CoroutineMan
    {
        class SequenceEntry
        {
            public Sequence Sequence;
            public bool AlwaysComplete;

            public SequenceEntry(Sequence sequence, bool alwaysComplete = false)
            {
                Sequence = sequence;
                AlwaysComplete = alwaysComplete;
            }
        }

        private readonly MonoBehaviour _monoBehaviour;

        private readonly Dictionary<string, SequenceEntry> _tweeners = new Dictionary<string, SequenceEntry>();
        private readonly Dictionary<string, (Coroutine Wrapper, Coroutine Real)> _coroutines = new Dictionary<string, (Coroutine, Coroutine)>();

        public CoroutineMan(MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }

        public bool IsActive(string key, CoroutineManType type = CoroutineManType.All)
        {
            if (type.HasFlag(CoroutineManType.Tweener))
            {
                if (_tweeners.ContainsKey(key))
                    return true;
            }
            if (type.HasFlag(CoroutineManType.Coroutine))
            {
                if (_coroutines.ContainsKey(key))
                    return true;
            }
            return false;
        }

        public void Register(string key, Tweener tw, bool alwaysComplete = false, bool unscaled = false)
        {
            Register(key, DOTween.Sequence().Append(tw), alwaysComplete: alwaysComplete, unscaled: unscaled);
        }

        public void Register(string key, Sequence seq, bool alwaysComplete = false, bool unscaled = false)
        {
            KillTweener(key);
            _tweeners.Add(key,
                new SequenceEntry(
                    seq.AppendCallback(() => _tweeners.Remove(key))
                    .SetUpdate(unscaled)
                    .Play(), alwaysComplete)
            );
        }

        public void Register(string key, IEnumerator coroutine)
        {
            KillCoroutine(key);
            var c = _monoBehaviour.StartCoroutine(coroutine);
            _coroutines.Add(key, (_monoBehaviour.StartCoroutine(WaitCoroutine(key, c)), c));
        }

        public void Register(string key, Coroutine coroutine)
        {
            KillCoroutine(key);
            _coroutines.Add(key, (_monoBehaviour.StartCoroutine(WaitCoroutine(key, coroutine)), coroutine));
        }

        public void KillAll(CoroutineManType killMode = CoroutineManType.All)
        {
            if (killMode.HasFlag(CoroutineManType.Tweener))
            {
                foreach (var twk in _tweeners.Keys.ToArray())
                    KillTweener(twk);
            }
            if (killMode.HasFlag(CoroutineManType.Coroutine))
            {
                foreach (var ck in _coroutines.Keys.ToArray())
                    KillCoroutine(ck);
            }
        }

        public void KillTweener(string key)
        {
            if (!_tweeners.TryGetValue(key, out var tw))
                return;
            if (tw.AlwaysComplete)
                tw.Sequence.Complete(true);
            else
                tw.Sequence.Kill();
            _tweeners.Remove(key);
        }

        public void KillCoroutine(string key)
        {
            if (!_coroutines.TryGetValue(key, out var c))
                return;
            _monoBehaviour.StopCoroutine(c.Wrapper);
            _monoBehaviour.StopCoroutine(c.Real);
            _coroutines.Remove(key);
        }

        private IEnumerator WaitCoroutine(string key, Coroutine coroutine)
        {
            yield return coroutine;
            _coroutines.Remove(key);
        }
    }
}
