using Sevens.Entities.Players;
using Sevens.Speeches;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnlockDash : MonoBehaviour
{
    public Player Player;

    public void Test()
    {
        DialogueManager.Instance.StartDialogue(Dialogue());
    }

    private IEnumerator Dialogue()
    {
        yield return new Dialogue("Hello", null, "hihihihi");
        yield return new Dialogue("Hello2", null, "hihihihi23123");
        yield return new Dialogue("Hello3", null, "hihihih12312321i");

        Player.Achievements.SetData(PlayerDataKeyType.UnlockedDash, "1");
    }
}
