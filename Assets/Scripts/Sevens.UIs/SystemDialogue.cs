using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sevens.Utils;

namespace Sevens.UIs
{
    public class SystemDialogueDisplayOption
    {
        public string Text { get; }
        public float Duration { get; } = 2.0f;

        public float FadeDuration = 0.5f;

        public SystemDialogueDisplayOption(string text)
        {
            Text = text;
        }

        public SystemDialogueDisplayOption(string text, float duration)
            : this(text)
        {
            Duration = duration;
            if (Duration < 0f)
                Duration = float.MaxValue;
        }
    }

    public class SystemDialogue : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _dialogue;

        private CoroutineMan _coroutine;

        private SystemDialogueDisplayOption _currentOption;

        private void Awake()
        {
            _coroutine = new CoroutineMan(this);
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        public void Display(string text)
        {
            Display(new SystemDialogueDisplayOption(text));
        }

        public void Display(string text, float duration)
        {
            Display(new SystemDialogueDisplayOption(text, duration));
        }

        public void Display(SystemDialogueDisplayOption option)
        {
            Debug.Log($"Called Display");

            var displayed = _coroutine.IsActive("Display");

            _coroutine.KillAll();

            var seq = DOTween.Sequence();
            if (displayed)
            {
                //_canvasGroup.alpha = 1f;
                seq.Append(FadeOut(_canvasGroup.alpha * _currentOption.FadeDuration));
            }

            if (option != null && !string.IsNullOrEmpty(option.Text))
            {
                seq.AppendCallback(() => _dialogue.text = _currentOption.Text)
                    .Append(FadeIn(option.FadeDuration))
                    .AppendInterval(option.Duration)
                    .Append(FadeOut(option.FadeDuration));
            }

            _coroutine.Register("Display", seq);

            _currentOption = option;
        }

        public void HideAll(float fadeDuration = 0.5f)
        {
            _coroutine.KillAll();
            _coroutine.Register("Display", 
                FadeOut(fadeDuration), 
                alwaysComplete: true, 
                unscaled: true
            );
        }

        private Tweener FadeOut(float time)
        {
            return _canvasGroup.DOFade(0f, time);
        }

        private Tweener FadeIn(float time)
        {
            return _canvasGroup.DOFade(1f, time);
        }
    }
}
