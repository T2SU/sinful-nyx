using Sevens.Entities.Players;
using System.Collections;
using UnityEngine;
using Sevens.Utils;
using UnityEngine.Serialization;
using DG.Tweening;
using UnityEngine.VFX;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_MagicGhost : MobAttackBase
    {
        [FormerlySerializedAs("Attack")]
        public GameObject AttackObj;
        public GameObject AttackPosition;
        public float AttackTimeScale;
        public float MagicSpeed;

        [SerializeField]
        private VisualEffect _attackVFX;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var pos = mob.transform.position;
            var animtime = mob.PlayAnimation(new AnimationPlayOption("Attack"), immediatelyTransition: true);
            mob.PlayAudio("Attack");
            _attackVFX.Play();

            var obj = Instantiate(AttackObj, AttackPosition.transform);
            obj.transform.parent = null;
            var rigid = obj.GetComponent<Rigidbody2D>();
            var velocity = Vector2.left * MagicSpeed * (mob.transform.IsFacingLeft() ? 1 : -1);

            coroutines.Register(
                "MagicAttackMove",
                DOTween.To(() => rigid.velocity, v => rigid.velocity = v, velocity, 0.3f)
                .SetEase(Ease.Linear)
            );

            yield return new WaitForSeconds(animtime);
            _attackVFX.Stop();

            mob.PlayAnimation(new AnimationPlayOption("Idle"));
            yield return new WaitForSeconds(AttackTimeScale - animtime);
        }
    }
}