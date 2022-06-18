using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sevens.Utils
{
    public class SceneLoadingComponent : MonoBehaviour
    {
        [SerializeField]
        private Text _progressText;
        private Color _progressTextColor;

        [SerializeField]
        private Image _progressBar;
        private Color _progressBarColor;

        [SerializeField]
        private Image _loadingSprite;
        private Color _loadingSpriteColor;

        public float Progress
        {
            set
            {
                if (_progressText != null)
                    _progressText.text = $"{Math.Ceiling(value * 100f):N0}%";
                if (_progressBar != null)
                    _progressBar.fillAmount = value;
            }
        }

        public void SetDone()
        {
            Fade(false, 1.0f);
        }

        private void Start()
        {
            StashColors();
            Fade(true, 0.25f);
        }

        private void StashColors()
        {
            if (_progressText != null)
                _progressTextColor = _progressText.color;
            if (_loadingSprite != null)
                _loadingSpriteColor = _loadingSprite.color;
            if (_progressBar != null)
                _progressBarColor = _progressBar.color;
        }

        private void Fade(bool firstStep, float duration)
        {
            void SetAlpha(Graphic g, Color c)
            {
                if (g != null)
                {
                    if (firstStep)
                        SetAlphaColor(g, 0f);
                    else
                        SetAlphaColor(g, c.a);
                }
            }

            SetAlpha(_progressText, _progressTextColor);
            // SetAlpha(_progressBar, _progressBarColor);
            SetAlpha(_loadingSprite, _loadingSpriteColor);

            void MakeFade(Graphic g, Color c)
            {
                if (g != null)
                {
                    if (firstStep)
                        g.DOColor(c, duration);
                    else
                        g.DOColor(Color.black, duration);
                }
            }

            MakeFade(_progressText, _progressTextColor);
            // MakeFade(_progressBar, _progressBarColor);
            MakeFade(_loadingSprite, _loadingSpriteColor);
        }

        private void SetAlphaColor(Graphic g, float val)
        {
            var cl = g.color;
            cl.a = val;
            g.color = cl;
        }
    }
}
