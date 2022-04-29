using Cinemachine;
using DG.Tweening;
using Sevens.Effects;
using Sevens.Utils;
#if UNITY_EDITOR
using Sevens.Utils.Editors;
#endif
using UnityEngine;

namespace Sevens.Cameras
{
    public enum CameraMode
    {
        Normal, Boss,
        Cutscene1, Cutscene2
    }

    public class VirtualCameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera _camera;

        [field: SerializeField]
#if UNITY_EDITOR
        [field: ShowCase]
#endif
        public CameraMode Mode { get; private set; }

        private CoroutineMan _coroutines;

        public void ReplaceMode(CameraMode mode, float duration, bool unscale = false)
        {
            if (Mode == mode)
                return;
            Mode = mode;
            var lens = _camera.m_Lens;
            Debug.Log($"Change camera mode {lens.OrthographicSize} -> {GetSizeForMode(mode)} ({mode}) {duration:F02}secs");
            _coroutines.Register("Mode", DOTween.To(
                () => lens.OrthographicSize,
                s => { lens.OrthographicSize = s; _camera.m_Lens = lens; },
                GetSizeForMode(mode),
                duration)
                .SetUpdate(unscale), unscaled: unscale);
        }

        public void Shake(CameraShakeOption opt)
        {
            Shake(opt.Amplitude, opt.Frequency, opt.Time);
        }

        public void Shake(float amplitude, float frequency, float time)
        {
            if (time <= 0)
                return;
            var perlin = _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _coroutines.Register("Shake", DOTween.Sequence()
                .AppendCallback(() => {
                    perlin.m_AmplitudeGain = amplitude;
                    perlin.m_FrequencyGain = frequency;
                })
                .AppendInterval(time)
                .AppendCallback(() =>
                {
                    perlin.m_AmplitudeGain = 0;
                    perlin.m_FrequencyGain = 0;
                })
            , alwaysComplete: true);
        }

        private float GetSizeForMode(CameraMode mode)
        {
            return mode switch
            {
                CameraMode.Boss => 7.6f,
                CameraMode.Cutscene1 => 4.4f,
                CameraMode.Cutscene2 => 5.0f,
                _ => 5.4f
            };
        }

        private void Awake()
        {
            Mode = CameraMode.Normal;
            _coroutines = new CoroutineMan(this);
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnDestroy()
        {
            _coroutines.KillAll();
        }
    }
}
