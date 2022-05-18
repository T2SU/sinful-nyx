using UnityEngine;
using UnityEngine.Events;

namespace Sevens.UIs
{
    public class TestDialogue : MonoBehaviour
    {
        public UnityEvent<string> OnInitiateDialogue;
        [SerializeField]
        private string _dialogue = "교수를 교수형에 처한다";

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnInitiateDialogue.Invoke(_dialogue);
        }
    }
}
