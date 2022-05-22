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
                SystemDialogue.Display("������ ������ ��ȣ�ۿ� �Ϸ��� <color=yellow>V</color> Ű�� �����ʽÿ�.", -1);
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
            yield return new Dialogue("������ ����", null, "�ȳ�? �װ� �������̱���.");
            yield return new Dialogue("������ ����", null, "...");
            yield return new Dialogue("������ ����", null, "�̰� ������. �� ��ȥ�̾�. �ʸ� �����ٰž�.");

            Player.Achievements.SetData(PlayerDataKeyType.UnlockedDash, "1");
            SystemDialogue.Display("<color=yellow>C</color> Ű�� ���� <color=skyblue>�뽬</color> ����� ����� �� �ֽ��ϴ�.", 4);
        }
    }

}
