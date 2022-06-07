using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


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
        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_AngryGuardian_JumpAttack);
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
                new AnimationPlayOption("Jump", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            coroutines.Register("JumpAttackMove", mob.transform.DOMove(transform.position + new Vector3(_jumpDistance * mob.GetFacingDirection(), _jumpHeight, 0), _jumpDuration));
            var targetPostion = GameObject.Find("Player").transform.position;
            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(attackManager);
            yield return new WaitForSeconds(_airborneTime);
            var obj = Instantiate(_jumpAttack, mob.transform.position, mob.transform.rotation);
            mob.PlayAnimation(
                new AnimationPlayOption("Jump", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            obj.transform.parent = mob.transform;
            coroutines.Register("JumpAttackMove", mob.transform.DOMove(targetPostion + new Vector3(0,3,0), _jumpDuration));
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            yield return new WaitForSeconds(_attackDuration);
            attackManager.EndAttack(false);
        }
    }
}

