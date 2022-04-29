using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAwakener : MonoBehaviour
    {
        [SerializeField]
        private Mob _mob;

        private int _playerLayer;

        private void Awake()
        {
            _playerLayer = LayerMask.NameToLayer("Player");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer != _playerLayer)
                return;
            if (_mob.State == MobState.Wait)
                _mob.ChangeState(MobState.Idle, playLoopAnimationByState: true);
            Destroy(gameObject);
        }
    }
}
