using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Entities;

public class GateEntity : InteractableEntity
{
    [SerializeField]
    private int gateIndex;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private LayerMask playerLayer;

    protected override void Update() {
        base.Update();
        if(Physics2D.OverlapCircle(transform.position, 4, playerLayer) && isClicked) {
            Interact();
        }
    }

    protected override void Interact() {
        _SceneManagement.Instance.LoadSceneByName(sceneName);
    }
}
