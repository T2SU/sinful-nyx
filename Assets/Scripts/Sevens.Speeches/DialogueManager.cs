﻿using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sevens.Speeches
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager _instance;
        private static GameObject _dialoguePrefab;

        public static DialogueManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("DialogueManager", typeof(DialogueManager));
                    _instance = obj.GetComponent<DialogueManager>();
                }
                return _instance;
            }
        }

        private Transform _dialogueObject;
        private Coroutine _dialogueCoroutine;
        private DialogueBase _dialogueBase;
        private Text _hudMessage;
        private CoroutineMan _coroutines;

        public DialogueBase DialogueBase => _dialogueBase;

        // 대화창 시작, 종료 메서드

        public Coroutine StartDialogue(IEnumerator dialogueCoroutine)
        {
            if (_dialogueObject != null)
                throw new InvalidOperationException("A dialogue has been already begun.");
            return StartCoroutine(DialogueManagementCoroutine(dialogueCoroutine));
        }

        public void StopDialogue()
        {
            if (_dialogueCoroutine == null)
                return;
            DeleteDialogue();
        }

        // 대화창이 열려있는지 여부. 추후 캐릭터 이동, 몬스터 피격 처리에 활용 가능?

        public bool HasDialogue()
        {
            return _dialogueObject != null;
        }

        // HUD 메시지
        public void DisplayHudMessage(string text, bool loop = false)
        {
            if (loop)
            {
                _coroutines.Register("HudMessage", DOTween.Sequence()
                    .AppendCallback(() => _hudMessage.text = text)
                    .AppendCallback(() => _hudMessage.color = new Color(1, 1, 1, 0))
                    .Append(_hudMessage.DOFade(1f, 0.75f))
                    .SetLoops(-1, LoopType.Yoyo));
            }
            else
            {
                _coroutines.Register("HudMessage", DOTween.Sequence()
                    .AppendCallback(() => _hudMessage.text = text)
                    .AppendCallback(() => _hudMessage.color = new Color(1, 1, 1, 0))
                    .Append(_hudMessage.DOFade(1f, 0.75f))
                    .Append(_hudMessage.DOFade(0f, 0.75f))
                    .Append(_hudMessage.DOFade(1f, 0.75f))
                    .Append(_hudMessage.DOFade(0f, 0.75f)));
            }
        }

        // 대화창 관련 코루틴 및 대화창 삭제 메서드

        private IEnumerator DialogueManagementCoroutine(IEnumerator dialogueCoroutine)
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (player != null)
                player.SetDirectionMode(true);
            _dialogueObject = Instantiate(_dialoguePrefab).transform;
            _dialogueBase = _dialogueObject.GetComponent<DialogueBase>();
            yield return _dialogueCoroutine = StartCoroutine(dialogueCoroutine);
            DeleteDialogue();
            if (player != null)
                player.SetDirectionMode(false);
        }

        private void DeleteDialogue()
        {
            Destroy(_dialogueObject.gameObject);
            _dialogueObject = null;
            _dialogueCoroutine = null;
        }

        // 유니티 이벤트

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _coroutines = new CoroutineMan(this);
            _dialoguePrefab = Resources.Load<GameObject>(Prefabs.Dialogue);
        }

    }
}
