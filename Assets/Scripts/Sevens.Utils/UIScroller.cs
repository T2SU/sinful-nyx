using DG.Tweening;
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

        [Header("Input")]
        public string ButtonName;

        [Header("Scroll")]
        public UIScrollDirection Direction = UIScrollDirection.Both;
        public Vector2 ScrollDelta;
        public float ScrollDuration;

        [Header("FadeImage")]
        public Image Image;

        public void PlayForward()
        {
            var dest = _originPosition + ScrollDelta;

            // 목적지로 좌표 이동
            _coroutines.Register("Scroll",
                _rectTransform.DOAnchorPos(dest, ScrollDuration)
                    , alwaysComplete: true
                    , unscaled: true);

            // 이미지 페이드 아웃
            if (Image != null)
            {
                _coroutines.Register("Fade",
                    Image.DOFade(1.0f, ScrollDuration)
                        , alwaysComplete: true
                        , unscaled: true);
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
            if (Image != null)
            {
                _coroutines.Register("Fade",
                    Image.DOFade(0.0f, ScrollDuration)
                        , alwaysComplete: true
                        , unscaled: true);
            }

            _displayed = false;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
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
