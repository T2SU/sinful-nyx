using Sevens.Entities.Players;
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

        public override void Cancel(MobAttackable attackManager)
        {
            attackManager.EndAttack(true);
            ClearObjects();
        }

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_AngryGuardian_ChargeAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            GameObject obj;
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("StampReady", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(attackManager);
            if (mob.IsFacingLeft())
            {
                obj = Instantiate(_fireBall, mob.transform.position, Quaternion.Euler(0, 0, (_fireDir - 180) * -1));
            }
            else
            {
                obj = Instantiate(_fireBall, mob.transform.position, Quaternion.Euler(0, 0, _fireDir));
            }
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            attackManager.EndAttack(false);
        }
    }
}
