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
    [SerializeField]
    private ScriptObject _introScript;

    private void Start() {
        if(Player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) != "1")
        {
            ToActivate?.SetActive(false);
            StartCoroutine(Dialogues());
        }
    }

    private IEnumerator Dialogues()
    {
        yield return DialogueManager.Instance.StartDialogue(_introScript.PlayScript());
        ToActivate?.SetActive(true);
        Player.SetDirectionMode(true);
        yield return new WaitForSeconds(4.0f);
        Player.SetDirectionMode(false);
    }

}
