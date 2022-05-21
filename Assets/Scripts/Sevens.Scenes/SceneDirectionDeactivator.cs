using DG.Tweening;
using Sevens.Cameras;
using Sevens.Effects;
using Sevens.Entities.Mobs;
using Sevens.Entities.Players;
using System.Collections;
using UnityEngine;

namespace Sevens.Scenes
{
    public class SceneDirectionDeactivator : MonoBehaviour
    {
        public GameObject CurtainBase;
        public Color CurtainColor;
        public float CurtainFadeOutDuration;
        public float BGMFadeInDuration;
        public CameraShakeOption CameraShake;
        public Mob WakeUp;
        public VirtualCameraController VirtualCamera;

        private AudioSource _bgm;
        private Player _player;

        public void Start()
        {
            if (GameObject.Find("SceneManagement") == null
                || _SceneManagement.Instance.sceneIsLoadedBySaved)
            {
                WakeUp?.ChangeState(MobState.Idle);
                Destroy(gameObject);
                return;
            }

            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            var cam = Camera.main;
            var curtain = Instantiate(CurtainBase, cam.transform);
            curtain.name = "SceneCurtain";
            var curtainSpr = curtain.GetComponent<SpriteRenderer>();
            curtainSpr.color = CurtainColor;
            curtainSpr.DOColor(Color.clear, CurtainFadeOutDuration)
                .SetDelay(2f);
            _player.SetDirectionMode(true);

            _bgm = GameObject.Find("BGM")?.GetComponent<AudioSource>();
            if (_bgm != null)
                _bgm.volume = 0f;
            StartCoroutine(Timeline());
        }

        private IEnumerator Timeline()
        {
            yield return new WaitForSeconds(2f);
            _player.SetDirectionMode(false);

            VirtualCamera.Shake(CameraShake);
            if (_bgm != null)
                _bgm.DOFade(1f, BGMFadeInDuration);
            yield return new WaitForSeconds(CurtainFadeOutDuration);
            yield return new WaitForSeconds(1.0f);
            if (WakeUp != null)
                WakeUp.ChangeState(MobState.Idle);
            Destroy(gameObject);
        }
    }
}
