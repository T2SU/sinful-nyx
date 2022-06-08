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

        [SerializeField]
        private LayerMask _groundLayer;

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
            var obj = Instantiate(_chargeAttack, mob.transform.position, mob.transform.rotation);
            obj.transform.parent = mob.transform;
            var hit = Physics2D.Raycast(mob.transform.position, Vector2.right * mob.GetFacingDirection(), _chargeDistance, _groundLayer);

            if (hit)
            {
                Debug.Log("notMaxDistanceChargeAttack");
                coroutines.Register("ChargeAttackMove", mob.transform.DOMove(transform.position + new Vector3(hit.distance * mob.GetFacingDirection(), 0, 0), _chargeDuration));
            }
            else
            {
                Debug.Log("MaxDistanceChargeAttack");
                coroutines.Register("ChargeAttackMove", mob.transform.DOMove(transform.position + new Vector3(_chargeDistance * mob.GetFacingDirection(), 0, 0), _chargeDuration));
            }
            //mob.PlayAudio("ChargeAttack");
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            yield return new WaitForSeconds(_chargeDuration);
            attackManager.EndAttack(false);
        }
    }
}

