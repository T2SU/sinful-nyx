using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_Plant : MobAttackBase
    {
        public GameObject Attack;
        public Transform AttackPoint;
        public float AttackTimeScale;
        [SerializeField]
        private float _height;
        private float gravity = 9.8f;
        private Ease ease;
        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_bat);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            attackManager.EndAttack(true);
            ClearObjects();
        }
        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;
            var player = attackManager.Player;
            var pos = mob.transform.position;

            var animtime = mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);
            var obj = Instantiate(Attack, AttackPoint);
            _objs.Add(obj);
            SetAllBlowSourceAs(obj, mob);
            mob.PlayAudio(nameof(Attack));
            var points = new Vector3[3];
            points.SetValue(obj.transform.position, 0);
            var midlePoint = new Vector3((player.transform.position.x + obj.transform.position.x) / 2, _height + obj.transform.position.y, 0);
            points.SetValue(midlePoint, 1);
            points.SetValue(player.transform.position, 2);
            obj.transform.DOPath(points, AttackTimeScale, PathType.CatmullRom, PathMode.Ignore)
                .SetLookAt(0f)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);

            yield return new WaitForSeconds(animtime);

            mob.PlayAnimation(new AnimationPlayOption("AfterAttack", timeScale: AttackTimeScale-animtime), immediatelyTransition: true);

            yield return new WaitForSeconds(AttackTimeScale - animtime);

            ClearObjects();
            attackManager.EndAttack(false);
        }

    }
}