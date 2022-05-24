using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Sevens.Entities.Players;

namespace Sevens.Speeches
{
    public class Dialogue
    {
        public string DialogueText { get; private set; }
        public string SpeakerName { get; private set; }
        public Sprite SpeakerAvatarSprite { get; private set; }

        private string[] _datalist;

        public Dialogue(string dialogueData) 
        {
            _datalist = dialogueData.Split('\t');
            
            SpeakerName = GetData(DialogueInfoType.Speaker);

            if (GetData(DialogueInfoType.Avatar) != "null")
            {
                SpeakerAvatarSprite = Resources.Load<Sprite>("Avatar/" + GetData(DialogueInfoType.Avatar));
            }
            else
            {
                SpeakerAvatarSprite = null;
            }

            DialogueText = GetData(DialogueInfoType.Text).Replace("\\n", "\n");
        }

        public DialogueRunner StartDialogue()
        {
            return new DialogueRunner(this);
        }

        private string GetData(DialogueInfoType type) => _datalist[(int)type];
    }
}
