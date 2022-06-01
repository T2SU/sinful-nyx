using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace Sevens.Entities.Mobs
{
    public class MobAttack_AngryGuardian_ChargeAttack : MobAttackBase
    {
        [SerializeField]
        private float _attackTimeScale;

        [SerializeField]
        private GameObject _chargeAttack;

        [SerializeField]
        private float _chargeDistance;

        [SerializeField]
        private float _chargeDuration;


        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_AngryGuardian_ChargeAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            attackManager.EndAttack(true);
            ClearObjects();
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("StampReady", timeScale: _attackTimeScale), 
                immediatelyTransition: true);

            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(attackManager);
            var hit = Physics2D.Raycast(transform.position, Vector2.right * mob.GetFacingDirection(), _chargeDistance, LayerMask.NameToLayer("Ground"));
            if (hit)
            {
                coroutines.Register("ChargeAttackMove", transform.DOMove(transform.position + new Vector3(hit.distance * mob.GetFacingDirection(), 0, 0), _chargeDuration));
            }
            else
            {
                coroutines.Register("ChargeAttackMove", transform.DOMove(transform.position + new Vector3(_chargeDuration * mob.GetFacingDirection(), 0, 0), _chargeDuration));
            }
            //mob.PlayAudio("ChargeAttack");
            var obj = Instantiate(_chargeAttack, mob.transform.position, mob.transform.rotation);
            obj.transform.parent = mob.transform;
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            yield return new WaitForSeconds(_chargeDuration);
            attackManager.EndAttack(false);
        }
    }
}

