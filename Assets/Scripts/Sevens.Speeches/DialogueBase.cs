// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sevens.Speeches
{
    public class DialogueBase : MonoBehaviour
    {
        [SerializeField]
        private SpeechSystem _speechSystem;

        [SerializeField]
        private Image _speakerAvatar;

        [SerializeField]
        private Text _speakerText;

        [SerializeField]
        private GameObject _letterPrefab;

        private bool _closed;

        public Font Font;
        public int FontSize;
        public Vector2 LetterSpacing;

        public bool Completed { get; private set; }
        public GameObject LetterPrefab => _letterPrefab;

        void Update()
        {
            // TODO keymapping through the Keysetting manager.
            if (Input.GetButtonDown("Fire1")) // Ȯ��
            {
                if (_speechSystem.CompleteSpeechForcely())
                    Completed = true;
            }
            else if (Input.GetButtonDown("DialogueSkip")) // �ݱ�
            {
                _closed = true;
                while (!_speechSystem.CompleteSpeechForcely()) ;
                Completed = true;
            }
        }

        public void SendSpeech(Dialogue dialogue)
        {
            if (_closed)
            {
                Completed = true;
                return;
            }
            if (!DialogueRunner.Running)
                throw new InvalidOperationException("Please use FieldManager.Instance.StartDialogue() method.");
            Completed = false;
            _speakerText.text = dialogue._speakerName;
            _speakerAvatar.sprite = dialogue._speakerAvatar;
            if (dialogue._speakerAvatar == null)
                _speakerAvatar.color = new Color(0, 0, 0, 0);
            else
                _speakerAvatar.color = Color.white;
            _speechSystem.DisplaySpeech(dialogue._dialogueText, Font, FontSize, LetterSpacing);
        }
    }
}
