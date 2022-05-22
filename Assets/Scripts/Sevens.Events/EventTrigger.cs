using Sevens.Entities.Players;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private string _buttonName;
        [SerializeField] private string _buttonKey;
        [SerializeField] private string _buttonDescription;

        private int _playerMask;
        private InteractionGuide _interactionGuide;
        private bool _alreadyTriggered;

        private bool IsStaying => _interactionGuide.gameObject.activeSelf;
        private Player _stayingPlayer;

        private bool _triggerEntered;

        private void Awake()
        {
            _playerMask = LayerMask.NameToLayer("Player");
            _alreadyTriggered = false;
            _interactionGuide = GetComponentInChildren<InteractionGuide>();
            _interactionGuide.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer != _playerMask)
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
                        break;
                    case InteractionType.PressInteractionKey:
                        _interactionGuide.gameObject.SetActive(true);
                        _interactionGuide.SetText(_buttonKey, _buttonDescription);
                        _stayingPlayer = player;
                        break;
                }
                if (!_triggerEntered)
                    OnTriggerEntered?.Invoke(player);
                _triggerEntered = true;

                _alreadyTriggered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer != _playerMask)
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
            if (!Input.GetButtonDown(_buttonName))
                return;
            OnInteraction?.Invoke(_stayingPlayer);
        }
    }
}
