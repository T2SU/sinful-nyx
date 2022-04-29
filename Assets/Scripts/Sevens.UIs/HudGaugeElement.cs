// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sevens.UIs
{
    public class HudGaugeElement : MonoBehaviour
    {
        [Serializable]
        public class LiquidTweener
        {
            [HideInInspector]
            public Material Material;

            public GameObject LiquidObject;

            private int _nameHash;
            private Tweener _tweener;

            public int NameHash
            {
                get
                {
                    if (_nameHash == 0)
                        _nameHash = Shader.PropertyToID("_GrowUp");
                    return _nameHash;
                }
            }

            public void KillTween()
            {
                if (_tweener == null)
                    return;
                _tweener.Kill();
                _tweener = null;
            }

            public void StartTween(float endValue, float duration)
            {
                KillTween();
                _tweener = Material.DOFloat(endValue, NameHash, duration)
                                        .OnKill(() => _tweener = null);
            }

            public float Get() => Material.GetFloat(NameHash);

            public void Set(float f)
            {
                if (Material == null)
                    return;
                KillTween();
                Material.SetFloat(NameHash, f);
            }
        }

        [SerializeField]
        private LiquidTweener _backLiquid;
        [SerializeField]
        private LiquidTweener _frontLiquid;

        public bool EnableAfterimageOnDecrease;
        public bool EnableAnimationOnIncrease;
        public float DecreaseDuration;
        public float IncreaseDuration;

        private void Awake()
        {
            void Dup(LiquidTweener obj)
            {
                var comp = obj.LiquidObject.GetComponent<Image>();
                var mat = new Material(comp.material);
                obj.Material = comp.material = mat;
            }
            Dup(_backLiquid);
            Dup(_frontLiquid);
        }

        public void SetGrowUpNoAnimation(float growUp)
        {
            _backLiquid.Set(growUp);
            _frontLiquid.Set(growUp);
        }

        public void SetGrowUp(float growUp)
        {
            var before = _frontLiquid.Get();
            if (growUp > before)
            {
                if (!EnableAnimationOnIncrease)
                {
                    SetGrowUpNoAnimation(growUp);
                    return;
                }
                _frontLiquid.StartTween(growUp, IncreaseDuration);
                _backLiquid.StartTween(growUp, IncreaseDuration);
            }
            else
            {
                if (!EnableAfterimageOnDecrease)
                {
                    SetGrowUpNoAnimation(growUp);
                    return;
                }
                _frontLiquid.Set(growUp);
                _backLiquid.StartTween(growUp, DecreaseDuration);
            }
        }

        public void OnPlayerDied(bool died)
        {
            if (died)
                gameObject.SetActive(false);
        }
    }
}
