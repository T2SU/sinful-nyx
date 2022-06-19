using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sevens.Utils;
using UnityEngine.Serialization;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_bat : MobAttackBase
    {
        public float AttackTimeScale;

        [FormerlySerializedAs("Attack")]
        public GameObject AttackObj;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var pos = mob.transform.position;

            mob.PlayAnimation(new AnimationPlayOption("Attack", timeScale: AttackTimeScale), immediatelyTransition: true);
            Instantiate(AttackObj, mob.transform);

            var seq = DOTween.Sequence()
                .AppendCallback(() => mob.PlayAudio("Attack"))
                .Append(mob.transform.DOMoveX(player.transform.position.x, AttackTimeScale))
                .AppendInterval(0.05f)
                .AppendCallback(() => mob.PlayAnimation(new AnimationPlayOption("AfterAttack", timeScale: AttackTimeScale), immediatelyTransition: true))
                .Append(mob.transform.DOMoveX(pos.x, AttackTimeScale * 2));
            coroutines.Register("BatAttack", seq);

            yield return new WaitForSeconds(AttackTimeScale * 2);
        }
    }
}