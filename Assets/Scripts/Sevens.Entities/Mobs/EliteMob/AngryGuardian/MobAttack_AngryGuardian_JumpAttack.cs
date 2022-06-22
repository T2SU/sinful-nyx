using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sevens.Utils;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_AngryGuardian_JumpAttack : MobAttackBase
    {
        [SerializeField]
        private float _attackTimeScale;

        [SerializeField]
        private float _jumpDistance;
        [SerializeField]
        private float _jumpHeight;
        [SerializeField]
        private float _jumpDuration;
        [SerializeField]
        private float _attackDuration;
        [SerializeField]
        private float _airborneTime;

        [SerializeField]
        private GameObject _jumpAttack;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Jump", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            mob.PlayAudio("Jump");
            coroutines.Register("JumpAttackMove", mob.transform.DOMove(transform.position + new Vector3(_jumpDistance * mob.GetFacingDirection(), _jumpHeight, 0), _jumpDuration));

            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(mob);
            var targetPostion = GameObject.Find("Player").transform.position;
            yield return new WaitForSeconds(_airborneTime);
            var obj = Instantiate(_jumpAttack, mob.transform.position, mob.transform.rotation);
            mob.PlayAnimation(
                new AnimationPlayOption("Landing", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            obj.transform.parent = mob.transform;
            coroutines.Register("JumpAttackMove", mob.transform.DOMove(targetPostion + new Vector3(0, 3, 0), _attackDuration));
            mob.PlayAudio("Landing");
            yield return new WaitForSeconds(_attackDuration);
        }
    }
}
