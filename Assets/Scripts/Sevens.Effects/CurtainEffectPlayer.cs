using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Sevens.Effects
{
    public class CurtainEffectPlayer : MonoBehaviour
    {
        public float Delay;
        public float Duration;
        public SpriteRenderer Curtain;
        public float EndValue;

        private void Start()
        {
            StartCoroutine(Animation());
        }

        private IEnumerator Animation()
        {
            yield return new WaitForSeconds(Delay);
            Curtain.DOFade(EndValue, Duration);
        }
    }
}
