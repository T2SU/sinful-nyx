using Sevens.Entities.Players;
using System.Collections;
using UnityEngine;
using Sevens.Utils;
using UnityEngine.Serialization;
using DG.Tweening;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_MagicGhost : MobAttackBase
    {
        [FormerlySerializedAs("Attack")]
        public GameObject AttackObj;
        public GameObject AttackPosition;
        public float AttackTimeScale;
        public float MagicSpeed;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var pos = mob.transform.position;
            var animtime = mob.PlayAnimation(new AnimationPlayOption("Attack"), immediatelyTransition: true);
            mob.PlayAudio("Attack");

            var obj = Instantiate(AttackObj, AttackPosition.transform);
            var rigid = obj.GetComponent<Rigidbody2D>();
            var velocity = Vector2.left * MagicSpeed * (mob.transform.IsFacingLeft() ? 1 : -1);

            coroutines.Register(
                "MagicAttackMove",
                DOTween.To(() => rigid.velocity, v => rigid.velocity = v, velocity, 0.3f)
                .SetEase(Ease.Linear)
            );

            yield return new WaitForSeconds(animtime);

            mob.PlayAnimation(new AnimationPlayOption("Idle"));
            yield return new WaitForSeconds(AttackTimeScale - animtime);
        }
    }
}