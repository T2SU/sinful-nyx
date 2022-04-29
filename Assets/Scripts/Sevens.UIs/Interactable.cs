using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Entities;

public class Interactable : InteractableEntity
{
    protected override void Interact()
    {
        UIUtility.Instance.OnPopUpInitiated(
        "상호작용 하시겠습니까?",
        "예",
        "아니오",
        () => { Debug.Log("작동!"); UIUtility.Instance.OnPopUpClosed(); },
        UIUtility.Instance.OnPopUpClosed
        );
    }
}
