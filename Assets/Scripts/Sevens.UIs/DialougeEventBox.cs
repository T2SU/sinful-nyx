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
        SystemDialogue.Display("<color=yellow>"+_command + "</color>Ű�� ���� <color=skyblue>"+ _action + "</color> �� �� �ֽ��ϴ�.", 2);
    }

}
