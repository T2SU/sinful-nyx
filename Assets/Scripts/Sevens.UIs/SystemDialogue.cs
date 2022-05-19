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

        [SerializeField]
        private float _displayDuration;

        private CoroutineMan _coroutine;

        private string _bufferedText;
        private float _expiryTime;

        private void Awake()
        {
            _coroutine = new CoroutineMan(this);
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (_expiryTime > 0 && _expiryTime <= Time.time)
            {
                _coroutine.Register("Fade", 
                    _canvasGroup.DOFade(0, _fadeDuration));
                _expiryTime = 0f;
            }
            if (_bufferedText != null && !_coroutine.IsActive("Fade"))
            {
                _dialogue.text = _bufferedText;
                _coroutine.Register("Fade", 
                    _canvasGroup.DOFade(1, _fadeDuration), 
                    alwaysComplete: true);
                _expiryTime = Time.time + _displayDuration + _fadeDuration;
                _bufferedText = null;
            }
        }

        public void Display(string text)
        {
            _bufferedText = text;
            if (_expiryTime > 0f)
                _expiryTime = Time.time;
        }
    }
}
