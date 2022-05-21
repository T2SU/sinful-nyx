using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Speeches;
using Sevens.Entities.Players;
using Sevens.Utils;

public class IntroDialogue : MonoBehaviour
{
    public GameObject ToActivate;
    public Player Player;

    private void Start() {
        if(Player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) != "1")
        {
            ToActivate?.SetActive(false);
            StartCoroutine(Dialogues());
        }
    }

    private IEnumerator Dialogues()
    {
        yield return DialogueManager.Instance.StartDialogue(Intro());
        Player.SetDirectionMode(true);
        yield return new WaitForSeconds(4.0f);
        Player.SetDirectionMode(false);
    }

    private IEnumerator Intro() {
        yield return new Dialogue("닉스", null, "…주인님?");
        yield return new Dialogue("닉스", null, "여긴… 어디?");
        ToActivate?.SetActive(true);
    }
}
