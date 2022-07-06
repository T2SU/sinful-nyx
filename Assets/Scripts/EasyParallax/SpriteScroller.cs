using UnityEngine;

namespace EasyParallax
{
    public class SpriteScroller : MonoBehaviour
    {
        public MovementSpeedType movementSpeedType;

        [Tooltip("Used only if no movement speed type is specified")]
        public float speed = 1f;

        private void Awake()
        {
            if (movementSpeedType)
                speed = movementSpeedType.speed;
        }

        public void Move(Vector3 delta)
        {
            var newPosition = transform.position;
            newPosition.x -= (speed * delta.x);
            transform.position = newPosition;
        }
    }
}