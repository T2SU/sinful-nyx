using UnityEngine;
using UnityEngine.Events;

namespace Sevens.UIs
{
    public class TestDialogue : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _layer;

        public UnityEvent<string> OnInitiateDialogue;
        [SerializeField]
        private string _dialogue = "교수를 교수형에 처한다";

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnInitiateDialogue.Invoke(_dialogue);
        }

        private void Update()
        {
            if(Physics2D.OverlapCircle(transform.position, 2f, _layer))
            {
                if(Input.GetKeyDown(KeyCode.V))
                {
                    _SceneManagement.Instance.SaveToJsonFile();
                }
            }
        }
    }
}
