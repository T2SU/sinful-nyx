using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sevens.Speeches
{
    [Serializable]
    class ScriptObject
    {
        [SerializeField]
        private TextAsset scriptFile;

        private string[] _dialogueInfos;
        private List<Dialogue> _dialogues;

        private void SplitScriptFile()
        {
            _dialogues = new List<Dialogue>();

            if (scriptFile != null)
            {
                _dialogueInfos = scriptFile.text.Split('\n');
                Debug.Log(_dialogueInfos.Length);
                for (int i = 0; i < _dialogueInfos.Length - 1; ++i)
                {
                    _dialogues.Add(new Dialogue(_dialogueInfos[i]));
                }
            }
            else
                _dialogues = null;
        }

        public IEnumerator PlayScript()
        {
            SplitScriptFile();

            if (_dialogues != null)
            {
                for (int i = 0; i < _dialogueInfos.Length - 1; ++i)
                    yield return _dialogues[i].StartDialogue();
            }
        }
    }
}
