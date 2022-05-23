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
        private string[] datalist;
        public string _dialogueText;
        public string _speakerName;
        public Sprite _speakerAvatar;

        public Dialogue(string dialogueData)
        {
            datalist = dialogueData.Split(',');
            
            _speakerName = GetData(DialogueInfoType.Speaker);

            if (GetData(DialogueInfoType.Avatar) != "null")
            {
                _speakerAvatar = Resources.Load<Sprite>(GetData(DialogueInfoType.Avatar));
            }
            else
            {
                _speakerAvatar = null;
            }

            _dialogueText = GetData(DialogueInfoType.Text);
        }

        private string GetData(DialogueInfoType type) => datalist[(int)type];
    }
}
