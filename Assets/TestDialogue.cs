using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Speeches;

public class TestDialogue : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        DialogueManager.Instance.StartDialogue(Test());
    }

    private IEnumerator Test() {
        //yield return new DialogueRunner("심영", null, "의사양반! 나 좀 살려주시오...");
        yield return null;
    }
}
