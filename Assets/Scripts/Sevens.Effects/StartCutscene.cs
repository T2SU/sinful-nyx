// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using Cinemachine;
using Cinemachine.PostFX;
using DG.Tweening;
using FunkyCode;
using Sevens.Cameras;
using Sevens.Utils;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sevens.Effects
{
    public class StartCutscene : MonoBehaviour
    {
        public SpriteRenderer Curtain;
        public SkeletonAnimation Animation;
        public ParticleSystem GlowParticle;
        public Light2D Light2D;
        public Transform Focus;
        public VirtualCameraController VCamController;
        public CinemachinePostProcessing CPP;
        public GameObject GlowNewEffect; // 90 Hit - Energy_41
        public string NextScene;
        public AudioClip GlowSound;
        public AudioSource Ambient;
        public Text Subtitle1;
        public Text Subtitle2;
        public Text Subtitle3;

        private Spine.AnimationState _state;
        private AudioSource _audioSource;
        private CoroutineMan _coroutine;

        private void Awake()
        {
            _state = Animation.AnimationState;
            _audioSource = GetComponent<AudioSource>();
            _coroutine = new CoroutineMan(this);
            Subtitle1.color = new Color(1, 1, 1, 0);
            Subtitle2.color = new Color(1, 1, 1, 0);

            var color = Subtitle3.color;
            color.a = 0f;
            Subtitle3.color = color;
        }

        private void Start()
        {
            StartCoroutine(Timeline());
        }

        private IEnumerator Timeline()
        {
            // 제발 눈을 떠 닉스
            yield return new WaitForSeconds(1.5f);

            bool seqEnd = false;
            _coroutine.Register("Subtitle1", 
                DOTween.Sequence()
                .Append(Subtitle1.DOFade(1.0f, 1.0f))
                .AppendInterval(2f)
                .Append(Subtitle1.DOFade(0.0f, 1.0f))
                .Append(Subtitle2.DOFade(1.0f, 1.0f))
                .AppendInterval(2f)
                .Append(Subtitle2.DOFade(0.0f, 1.0f))
                .AppendCallback(() => seqEnd = true));
            yield return new WaitUntil(() => seqEnd);

            yield return new WaitForSeconds(0.5f);
            Ambient.Play();
            yield return new WaitForSeconds(1f);

            // 위로 카메라 스르륵 이동
            Focus.DOMove(new Vector3(0, 0, 0), 9f);
            yield return new WaitForSeconds(9f);

            // 닉스 얼굴에서 빛이 반짝 + 사운드 + 살살 줌 아웃
            DOTween.To(() => Light2D.size, f => Light2D.size = f, 6f, 2f);
            _audioSource.PlayOneShot(GlowSound);
            Ambient.DOFade(0f, 1.0f);
            var newObj = Instantiate(GlowNewEffect, GlowParticle.transform.position, Quaternion.identity, Focus.transform);
            StopParticle(GlowParticle);
            DOTween.To(() => Light2D.size, f => Light2D.size = f, 10f, 5f);
            yield return new WaitForSeconds(1f);

            // 움직이다가, 눈을 뜸
            //VCamController.ReplaceMode(CameraMode.Cutscene1, 2.5f, false);
            _state.SetAnimation(0, "animation", false);
            VCamController.ReplaceMode(CameraMode.Cutscene2, 4f, false);

            // 화면 줌아웃 되다가, 화면을 하얗게 세게 함.
            yield return new WaitForSeconds(2.5f);
            StopParticle(newObj.GetComponent<ParticleSystem>());
            _coroutine.Register("Curtain", Curtain.DOColor(new Color(1, 1, 1, 0.3f), 3f));
            CPP.enabled = true;
            VCamController.ReplaceMode(CameraMode.Normal, 20f);

            // 화면을 하얗게 셈
            _coroutine.Register("Curtain", Curtain.DOColor(new Color(1, 1, 1, 1f), 2f));
            yield return new WaitForSeconds(2.25f);
            Subtitle3.DOFade(1.0f, 0.5f);
            yield return new WaitForSeconds(2f);
            Subtitle3.DOFade(0.0f, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // 다시 까맣게
            _coroutine.Register("Curtain", Curtain.DOColor(new Color(0, 0, 0, 1f), 4.5f));
            yield return new WaitForSeconds(4.5f);

            // Load Scene
            if (!string.IsNullOrEmpty(NextScene))
                SceneManager.LoadScene(NextScene);
        }

        private void StopParticle(ParticleSystem ps)
        {
            if (!ps) return;
            if (ps.isPlaying)
                ps.Stop(true);
        }
    }
}
