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

        public void Interaction()
        {
            DialogueManager.Instance.StartDialogue(Dialogue());
        }

        public void DisplaySystemDialogue()
        {
            if (Player.Achievements.GetData(PlayerDataKeyType.UnlockedDash) != "1")
            {
                SystemDialogue.Display("버려진 인형과 상호작용 하려면 <color=yellow>V</color> 키를 누르십시오.", -1);
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
            yield return new Dialogue("버려진 인형", null, "안녕? 네가 마지막이구나.");
            yield return new Dialogue("버려진 인형", null, "...");
            yield return new Dialogue("버려진 인형", null, "이걸 가져가. 내 영혼이야. 너를 도와줄거야.");

            Player.Achievements.SetData(PlayerDataKeyType.UnlockedDash, "1");
            SystemDialogue.Display("<color=yellow>C</color> 키를 눌러 <color=skyblue>대쉬</color> 기술을 사용할 수 있습니다.", 4);
        }
    }

}
