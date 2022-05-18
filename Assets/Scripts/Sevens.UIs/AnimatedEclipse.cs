using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Sevens.UIs
{
    public class AnimatedEclipse : MonoBehaviour
    {
        [SerializeField] 
        private Image _shadow;

        [SerializeField]
        private float _moveDuration;

        [SerializeField]
        private Transform _fullTransform;
        [SerializeField]
        private Transform _emptyTransform;

        private Tweener _tweener;

        public void UpdateEclipse(float ratio)
        {
            _shadow.transform.DOMove(CalcNewPosition(ratio), _moveDuration)
                .SetEase(Ease.Linear);
        }

        public void SetEclipse(float ratio)
        {
            _shadow.transform.position = CalcNewPosition(ratio);
        }

        private Vector3 CalcNewPosition(float ratio)
            => _emptyTransform.position + (_fullTransform.position - _emptyTransform.position) * ratio;
    }
}