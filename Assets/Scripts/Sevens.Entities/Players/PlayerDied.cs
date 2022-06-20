using DG.Tweening;
using Sevens.Cameras;
using Sevens.Effects;
using Sevens.Entities.Mobs;
using Sevens.Speeches;
using Sevens.UIs;
using Sevens.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sevens.Interfaces;

namespace Sevens.Entities.Players
{
    public class PlayerDied : MonoBehaviour
    {
        public AudioClip DyingBGM;

        private VirtualCameraController _vcam;
        private AudioSource _bgm;
        private CoroutineMan _coroutines;
        private bool _isWaitingRestartGame;
        private GameObject _ui;

        private void Awake()
        {
            _vcam = GameObject.FindObjectOfType<VirtualCameraController>();
            _bgm = GameObject.Find("BGM")?.GetComponent<AudioSource>();
            _coroutines = new CoroutineMan(this);
            _ui = GameObject.Find("UI");
        }

        public void OnDied()
        {
            StopAllMobs();
            foreach (var hud in _ui.GetComponentsInChildren<AnimatedGauge>())
                hud.OnPlayerDied(true);
            _vcam.Shake(new CameraShakeOption() { Amplitude = 0.0f, Frequency = 0.0f, Time = 0.1f });
            _coroutines.Register("BGMFadeOut", DOTween.Sequence()
                .Append(_bgm.DOFade(0f, 2f))
                .AppendInterval(1.5f)
                .AppendCallback(() => { _bgm.clip = DyingBGM; _bgm.Play(); })
                .Append(_bgm.DOFade(1f, 0.25f))
                .AppendInterval(1.5f)
                .AppendCallback(() => {
                _isWaitingRestartGame = true;
                //DialogueManager.Instance.DisplayHudMessage("마지막 체크포인트로: Z | 타이틀 화면으로: X", true);
                UIManager.Instance.Popup("YOU DIED",
                    "마지막 체크포인트로",
                    "타이틀 화면으로",
                    () => { SceneManagement.Instance.LoadGame(); },
                    () => { SceneManagement.Instance.LoadScene("Main Menu_New");
                    });
                }), unscaled: true
            );
        }

        private void StopAllMobs()
        {
            var objs = FindObjectsOfType<Mob>();
            foreach (var obj in objs)
            {
                var attackable = obj.GetComponent<MobAttackable>();
                var moveable = obj.GetComponent<MobMovable>();
                if (attackable != null)
                {
                    attackable.CancelAttack();
                    attackable.enabled = false;
                }
                if (moveable != null)
                {
                    moveable.enabled = false;
                }
            }
        }

       /*private void Update()
        {
            if (_isWaitingRestartGame)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    SceneManager.LoadScene("Main Menu_New");
                }
                else if (Input.GetButtonDown("Fire1"))
                {
                    SceneManagement.Instance.LoadGame();
                }
            }
        }*/
    }
}
