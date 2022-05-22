using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.UIs;

public class DialougeEventBox : MonoBehaviour
{
    public SystemDialogue SystemDialogue;
    [SerializeField]
    private string _command;
    [SerializeField]
    private string _action;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SystemDialogue.Display("<color=yellow>"+_command + "</color>키를 눌러 <color=skyblue>"+ _action + "</color> 할 수 있습니다.", 2);
    }

}
