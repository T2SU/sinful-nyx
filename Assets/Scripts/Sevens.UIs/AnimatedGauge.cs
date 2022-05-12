using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Sevens.UIs
{
    public class AnimatedGauge : MonoBehaviour
    {
        [SerializeField]
        private Image _barMainImage;

        [SerializeField]
        private Image _barSubImage;

        [SerializeField]
        private float _fillDuration;

        private Tweener _tweener;
        private float _lastRatio;

        public void UpdateBar(float ratio)
        {
            if (_lastRatio > ratio)
            {
                // 줄어드는거
                _barMainImage.fillAmount = ratio;
                _tweener = _barSubImage.DOFillAmount(ratio, _fillDuration);
            }
            else
            {
                // 회복되는거
                _barMainImage.DOFillAmount(ratio, _fillDuration);
                _barSubImage.DOFillAmount(ratio, _fillDuration);
            }
            _lastRatio = ratio;
        }

        public void SetBar(float ratio)
        {
            _barMainImage.fillAmount = ratio;
            _barSubImage.fillAmount = ratio;
            _lastRatio = ratio;
        }
    }
}