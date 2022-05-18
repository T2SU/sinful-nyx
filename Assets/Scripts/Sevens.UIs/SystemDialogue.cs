using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sevens.Utils;

namespace Sevens.UIs
{
    public class SystemDialogue : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _dialogue;

        [SerializeField]
        private float _fadeDuration;

        private CoroutineMan _coroutine;

        private void Awake()
        {
            _coroutine = new CoroutineMan(this);
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        public void Display(string text)
        {
            /*
             - 기존에 텍스트가 있으면 즉시 없애기
               - 기존 대기 시간을 즉시 만료 (페이드 아웃 애니메이션 존재)
               - 그냥 바로 페이드 아웃 없이 삭제 (페이드 아웃 애니메이션 없음)
             - 새로운 텍스트를 추가
             - 페이드 인
            */
        }

        public void InitiateDialogue(string dialogueText)
        {
            _dialogue.text = dialogueText;
            _canvasGroup.DOFade(1, _fadeDuration);
        }
    }
}
