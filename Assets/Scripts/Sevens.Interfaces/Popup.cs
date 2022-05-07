using DG.Tweening;
using Sevens.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sevens.Interfaces
{
    public class Popup : MonoBehaviour
    {
        private enum PopupDirection
        {
            Show = 1, Hide = 0
        }

        [SerializeField]
        private GameObject _popup;

        [SerializeField]
        private Transform _popupText;

        [SerializeField]
        private Transform[] _selections;

        [SerializeField]
        private float _fadeDuration;

        private PopupWait _popupResult = new PopupWait();
        private CoroutineMan _coroutines;
        private CanvasGroup _canvasGroup;

        public void SetText(string text)
        {
            var popupText = _popupText.GetComponentInChildren<Text>();
            popupText.text = text;
        }

        public void SetSelectionText(int index, string text)
        {
            var selectionText = _selections[index].GetComponentInChildren<Text>();
            selectionText.text = text;
        }

        public void AddClickListener(int index, UnityAction action)
        {
            var btn = _selections[index].GetComponent<Button>();
            btn.onClick.AddListener(action);
        }

        public PopupWait Show()
        {
            for (int i = 0; i < _selections.Length; i++)
            {
                int index = i;
                AddClickListener(index, () => OnClicked(index));
            }
            _popup.SetActive(true);
            RegisterFadeWithDestroy(PopupDirection.Show, afterDestroy: false);
            return _popupResult;
        }

        private void OnClicked(int i)
        {
            _popupResult.Selected = true;
            _popupResult.SelectedIndex = i;
            RemoveAllListeners();
            RegisterFadeWithDestroy(PopupDirection.Hide, afterDestroy: true);
        }

        private void RemoveAllListeners()
        {
            _canvasGroup.interactable = false;
            foreach (var sel in _selections)
            {
                var btn = sel.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
            }
        }

        private void RegisterFadeWithDestroy(PopupDirection dir, bool afterDestroy)
        {
            // 페이드 애니메이션
            _canvasGroup.alpha = Convert.ToInt32(dir) ^ 1;
            var tw = _canvasGroup.DOFade(Convert.ToInt32(dir), _fadeDuration);

            // 애니메이션 종료 후 오브젝트 삭제 여부
            if (afterDestroy)
                tw.OnComplete(() => Destroy(gameObject));

            // DOTween 등록
            _coroutines.Register("Fade", tw, 
                alwaysComplete: true, 
                unscaled: true);
        }

        private void Awake()
        {
            _coroutines = new CoroutineMan(this);
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            if (!_popupResult.Selected)
                _popupResult.Selected = true;
        }
    }
}
