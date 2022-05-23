using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sevens.Speeches
{
    class ScriptData 
    {
        [SerializeField]
        private TextAsset scriptFile;

        private string[] dialogueInfos;
        private Dialogue[] dialogues;

        public bool IsEnd = false;

        public ScriptData(TextAsset textAsset)
        {
            if (textAsset != null)
            {
                scriptFile = textAsset;
            }
            else
                return;

            dialogueInfos = scriptFile.text.Split('\n');
            for (int i = 0; i < dialogueInfos.Length; ++i)
            {
                dialogues[i] = new Dialogue(dialogueInfos[i]);
                Debug.Log(dialogues.Length);
            }
        }

        public IEnumerator StartConversation()
        {
            for (int i = 0; i < dialogues.Length; ++i)
                yield return new DialogueRunner(dialogues[i]);
            IsEnd = true;
        }
    }
}
