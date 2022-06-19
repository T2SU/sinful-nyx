using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sevens.Utils;
using UnityEngine.Serialization;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_Plant : MobAttackBase
    {
        [FormerlySerializedAs("Attack")]
        public GameObject AttackObj;
        public Transform AttackPoint;
        public float AttackTimeScale;
        [SerializeField]
        private float _height;
        private Ease ease;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var animtime = mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);
            var obj = Instantiate(AttackObj, AttackPoint);
            mob.PlayAudio("Attack");
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

            mob.PlayAnimation(new AnimationPlayOption("AfterAttack", timeScale: AttackTimeScale - animtime), immediatelyTransition: true);

            yield return new WaitForSeconds(AttackTimeScale - animtime);
        }
    }
}