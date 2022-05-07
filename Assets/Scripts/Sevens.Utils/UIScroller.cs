using DG.Tweening;
using Sevens.Speeches;
using UnityEngine;
using UnityEngine.UI;

namespace Sevens.Utils
{
    public enum UIScrollDirection
    {
        Forward = 1 << 0,
        Backward = 1 << 1,
        Both = Forward | Backward
    }

    public class UIScroller : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private bool _displayed;
        private Vector2 _originPosition;
        private CoroutineMan _coroutines;
        private CanvasGroup _canvasGroup;

        [Header("Input")]
        public string ButtonName;
        public bool IgnoredIfHasDialogue;

        [Header("Scroll")]
        public UIScrollDirection Direction = UIScrollDirection.Both;
        public Vector2 ScrollDelta;
        public float ScrollDuration;

        public bool Displayed => _displayed;

        public void PlayForward()
        {
            if (IgnoredIfHasDialogue)
            {
                if (DialogueManager.Instance.HasDialogue())
                    return;
            }

            var dest = _originPosition + ScrollDelta;

            // 목적지로 좌표 이동
            _coroutines.Register("Scroll",
                _rectTransform.DOAnchorPos(dest, ScrollDuration)
                    , alwaysComplete: true
                    , unscaled: true);

            // 이미지 페이드 아웃
            if (_canvasGroup != null)
            {
                _coroutines.Register("Fade",
                    _canvasGroup.DOFade(1.0f, ScrollDuration)
                        , alwaysComplete: true
                        , unscaled: true);
                _canvasGroup.interactable = true;
            }

            _displayed = true;
        }

        public void PlayBackward()
        {
            var dest = _originPosition;

            // 원래 위치로 좌표 이동
            _coroutines.Register("Scroll",
                _rectTransform.DOAnchorPos(dest, ScrollDuration)
                    , alwaysComplete: true
                    , unscaled: true);

            // 이미지 페이드 인
            if (_canvasGroup != null)
            {
                _coroutines.Register("Fade",
                    _canvasGroup.DOFade(0.0f, ScrollDuration)
                        , alwaysComplete: true
                        , unscaled: true);
                _canvasGroup.interactable = false;
            }

            _displayed = false;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _displayed = false;
            _originPosition = _rectTransform.anchoredPosition;
            _coroutines = new CoroutineMan(this);
        }

        private void Update()
        {
            if (Input.GetButtonDown(ButtonName))
            {
                if (_displayed)
                {
                    if (Direction.HasFlag(UIScrollDirection.Backward))
                        PlayBackward();
                }
                else
                {
                    if (Direction.HasFlag(UIScrollDirection.Forward))
                        PlayForward();
                }
            }
        }
    }
}
