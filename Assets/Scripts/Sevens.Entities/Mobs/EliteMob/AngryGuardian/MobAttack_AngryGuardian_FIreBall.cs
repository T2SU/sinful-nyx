using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Sevens.Entities.Mobs
{
    public class MobAttack_AngryGuardian_FIreBall : MobAttackBase
    {
        [SerializeField]
        private float _attackTimeScale;

        [SerializeField]
        private GameObject _fireBall;

        [SerializeField]
        private float _fireDir;

        [SerializeField]
        private float _attackDuration;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("FireBall", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(mob);
            if (mob.IsFacingLeft())
            {
                Instantiate(_fireBall, mob.transform.position, Quaternion.Euler(0, 0, (_fireDir - 180) * -1));
            }
            else
            {
                Instantiate(_fireBall, mob.transform.position, Quaternion.Euler(0, 0, _fireDir));
            }
            yield return new WaitForSeconds(_attackDuration);
        }
    }
}
