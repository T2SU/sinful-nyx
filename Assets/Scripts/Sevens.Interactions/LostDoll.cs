using Sevens.Entities.Players;
using Sevens.Speeches;
using Sevens.UIs;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sevens.Interactions
{
    public class LostDoll : MonoBehaviour
    {
        public Player Player;
        public SystemDialogue SystemDialogue;

        [SerializeField]
        private ScriptObject _script;

        public void Interaction()
        {
            if(!_script.AleadyExcuted)
                StartCoroutine(Dialogue());
        }

        public void DisplaySystemDialogue()
        {
            if (Player.Achievements.GetData(PlayerDataKeyType.UnlockedDash) != "1")
            {
                SystemDialogue.Display("상호작용 하려면 <color=yellow>V</color> 키를 누르십시오.", -1);
            }
        }

        public void HideSystemDialogue()
        {
            if (Player.Achievements.GetData(PlayerDataKeyType.UnlockedDash) != "1")
            {
                SystemDialogue.Display(null, 0);
            }
        }

        private IEnumerator Dialogue()
        {
            yield return DialogueManager.Instance.StartDialogue(_script.PlayScript());
            Player.Achievements.SetData(PlayerDataKeyType.UnlockedDash, "1");
            SystemDialogue.Display("<color=yellow>C</color> 키를 눌러 <color=skyblue>대쉬</color> 기술을 사용할 수 있습니다.", 4);
        }
    }

}
