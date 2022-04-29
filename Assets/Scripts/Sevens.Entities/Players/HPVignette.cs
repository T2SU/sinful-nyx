using DG.Tweening;
using Sevens.Utils;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Sevens.Entities
{
    public class HPVignette : MonoBehaviour
    {
        [SerializeField]
        private PostProcessProfile _hpVignetteProfile;

        [SerializeField]
        private PostProcessVolume processVolume;

        private CoroutineMan _coroutines;

        [SerializeField]
        private float _duration;
        private Vignette _vignette;

        [SerializeField]
        private float _yoyoDuration;

        private float _currentValue = 0f;

        private Tweener _tweener;

        private void Awake()
        {
            _coroutines = new CoroutineMan(this);
            _vignette = processVolume.profile.GetSetting<Vignette>();
        }

        public void ChangedHPRatio(float ratio)
        {
            if (ratio <= 0f)
                ChangeIntensity(0f);
            else if (ratio < 0.1f)
                ChangeIntensity(.6f);
            else if (ratio < 0.2f)
                ChangeIntensity(.55f);
            else if (ratio < 0.3f)
                ChangeIntensity(.5f);
            else if (ratio < 0.4f)
                ChangeIntensity(.45f);
            else if (ratio < 0.5f)
                ChangeIntensity(.4f);
            else if (ratio < 0.6f)
                ChangeIntensity(.3f);
            else
                ChangeIntensity(0f);
        }

        public void ChangeIntensity(float value)
        {
            if (_currentValue == value)
                return;
            _currentValue = value;
            _coroutines.Register("HPVignette",
                DOTween.To(
                    () => _vignette.intensity.value,
                    i => _vignette.intensity.Override(i),
                    value,
                    _duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (_tweener != null)
                        _tweener.Kill();
                    _tweener = DOTween.To(
                            () => _vignette.intensity.value,
                            i => _vignette.intensity.Override(i),
                            _vignette.intensity.value * 0.75f,
                            _yoyoDuration)
                        .SetDelay(_duration)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Yoyo);
                }), alwaysComplete: true);
        }
    }

}