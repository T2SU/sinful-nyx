using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Entities;
using Sevens.Speeches;
using Sevens.Entities.Players;
using Sevens.Utils;
using Sevens.Interfaces;
using Sevens.UIs;
using Sevens.Events;

public class SavePointEntity : MonoBehaviour
{
    private bool _dialogueAlreadyExists;

    [SerializeField]
    private GameObject particleBlink;
    [SerializeField]
    private GameObject particleGlow;
    private DialogueBase dialogueBase;
    [SerializeField]
    private LayerMask playerLayer;

    [SerializeField] Player player;
    [SerializeField] Sprite _angelAvatar;
    [SerializeField] SystemDialogue _systemDialogue;

    [SerializeField]
    private ScriptObject _scripts;

    private void Start()
    {
        var playerObj = GameObject.Find("Player");

        if (playerObj == null)
        {
            return;
        }

        player = playerObj.GetComponent<Player>();

        if (player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) == "1")
        {
            ChangeButtonTooltip();
        }
    }

    public void Interact() {
        if (player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) != "1")
        {
            StartCoroutine(FirstContactDialogue());
        }
        else 
        {
            UIManager.Instance.Popup("저장 하시겠습니까?", "예", "아니오", () => {
                SaveManager.SaveGame();
                DialogueManager.Instance.DisplayHudMessage("데이터가 저장되었습니다.");
            });
        }
    }

    public void DisplaySystemDialogue()
    {
        if (player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) != "1")
        {
            _systemDialogue.Display("상호작용 하려면 <color=yellow>V</color> 키를 누르십시오.", -1);
        }
    }

    public void HideSystemDialogue()
    {
        if (player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) != "1")
        {
            _systemDialogue.Display(null, 0);
        }
    }

    private IEnumerator FirstContactDialogue()
    {
        yield return DialogueManager.Instance.StartDialogue(_scripts.PlayScript());
        yield return DelayedParticle();
        player.Achievements.SetData(PlayerDataKeyType.FirstContactCompleted, "1");
        ChangeButtonTooltip();
        _systemDialogue.Display(null, -1);
    }


    private void ChangeButtonTooltip()
    {
        var et = transform.GetComponentInChildren<EventTrigger>();
        et.ButtonDescription = "저장";
    }
}
