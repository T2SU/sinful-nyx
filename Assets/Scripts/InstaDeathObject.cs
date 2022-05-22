using UnityEngine;
using Sevens.Entities.Players;

namespace Scripts
{
    public class InstaDeathObject : MonoBehaviour
    {
        [SerializeField]
        private Player _player;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _player.Hp = 0;
        }
    }
}
