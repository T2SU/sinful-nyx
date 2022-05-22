using Sevens.Entities.Players;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Sevens.Events
{
    public enum InteractionType
    {
        AutorunOnEntered,
        PressInteractionKey
    }

    public class EventTrigger : MonoBehaviour
    {
        public UnityEvent<Player> OnInteraction;
        public UnityEvent<Player> OnTriggerEntered;
        public UnityEvent<Player> OnTriggerExited;

        [SerializeField] private InteractionType _interactionType;
        [SerializeField] private bool _preserveTrigger;

        [FormerlySerializedAs("_buttonName")]
        public string ButtonName;

        [FormerlySerializedAs("_buttonKey")]
        public string ButtonKey;

        [FormerlySerializedAs("_buttonDescription")]
        public string ButtonDescription;

        private int _playerLayer;
        private InteractionGuide _interactionGuide;
        private bool _alreadyTriggered;

        private bool IsStaying => _interactionGuide.gameObject.activeSelf;
        private Player _stayingPlayer;

        private bool _triggerEntered;

        private void Awake()
        {
            _playerLayer = LayerMask.NameToLayer("Player");
            _alreadyTriggered = false;
            _interactionGuide = GetComponentInChildren<InteractionGuide>();
            _interactionGuide.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer != _playerLayer)
                return;
            if (!_preserveTrigger && _alreadyTriggered)
                return;
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                switch (_interactionType)
                {
                    case InteractionType.AutorunOnEntered:
                        OnInteraction?.Invoke(player);
                        _alreadyTriggered = true;
                        break;
                    case InteractionType.PressInteractionKey:
                        _interactionGuide.gameObject.SetActive(true);
                        _interactionGuide.SetText(ButtonKey, ButtonDescription);
                        _stayingPlayer = player;
                        break;
                }
                if (!_triggerEntered)
                    OnTriggerEntered?.Invoke(player);
                _triggerEntered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer != _playerLayer)
                return;
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                switch (_interactionType)
                {
                    case InteractionType.PressInteractionKey:
                        _interactionGuide.gameObject.SetActive(false);
                        _stayingPlayer = null;
                        break;
                }

                if (_triggerEntered)
                    OnTriggerExited?.Invoke(player);
                _triggerEntered = false;
            }
        }

        private void Update()
        {
            if (_interactionType != InteractionType.PressInteractionKey)
                return;
            if (!IsStaying)
                return;
            if (!Input.GetButtonDown(ButtonName))
                return;
            OnInteraction?.Invoke(_stayingPlayer);
            _alreadyTriggered = true;
        }
    }
}
