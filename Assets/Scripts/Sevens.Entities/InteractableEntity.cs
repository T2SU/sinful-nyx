using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sevens.Entities
{
    public class InteractableEntity : Entity
    {
        [field:SerializeField]
        public Collider2D InteractableZone { get; protected set; }

        private Transform toolTip;
        [SerializeField]
        private Transform toolTipPrefab;

        [SerializeField]
        protected bool isClicked;

        protected override void Update()
        {
            isClicked = Input.GetKeyDown(KeyCode.V);
        }

        protected virtual void Interact()
        {
            Debug.Log("Interacted");
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(toolTip == null) {
                toolTip = Instantiate(toolTipPrefab, new Vector2(InteractableZone.bounds.center.x, InteractableZone.bounds.center.y + 0.5f), Quaternion.identity);
            }

            toolTip.gameObject.SetActive(true);
        }

        private void OnTriggerStay2D(Collider2D other) {
            if(other != null && other.CompareTag("Player")) {
                if(isClicked) {
                    Interact();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(toolTip != null && toolTip.gameObject.activeSelf) {
                toolTip.gameObject.SetActive(false);
            }
        }
    }
}
