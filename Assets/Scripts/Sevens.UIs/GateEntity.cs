using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Entities;
using Sevens.Utils;
using Sevens.Entities.Players;
using Sevens.Speeches;

public class GateEntity : InteractableEntity
{
    [SerializeField]
    private int gateIndex;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private ScriptObject _lockedDialogue;

    protected override void Update() {
        base.Update();
        if(Physics2D.OverlapCircle(transform.position, 4, playerLayer) && isClicked) 
        {
            if (_player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) != "1")
            {
                DialogueManager.Instance.StartDialogue(_lockedDialogue.PlayScript());
            }
            else
            {
                Interact();
            }
        }
    }

    protected override void Interact() {
        SceneManagement.Instance.LoadScene(sceneName);
    }
}
