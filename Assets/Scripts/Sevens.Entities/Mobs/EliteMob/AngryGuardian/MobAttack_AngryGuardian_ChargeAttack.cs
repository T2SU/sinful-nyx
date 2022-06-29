using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sevens.Utils;
using UnityEngine.VFX;

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

        [SerializeField]
        private VisualEffect _chargeAttackVFX;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("StampReady", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            _chargeAttackVFX.Play();

            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(mob);
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
            mob.PlayAudio("ChargeAttack");
            yield return new WaitForSeconds(_chargeDuration);
        }
    }
}

