using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_RandomTargetAttack : MobAttackBase
    {
        public GameObject TargetIndicator;
        public GameObject FireBlast;
        public Collider2D SpawnRange;

        public float TargetSpawnDelay;
        public int AttackCount;
        public float AttackInterval;
        public float AttackTimeScale;
        public string BaseTrembleEffectName;
        public string TrembleEffectName;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase2_RandomTargetAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(true);
            ClearObjects();
            mob.Invincible = false;
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;

            mob.PlayAnimation(
                new AnimationPlayOption("Skill3", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            yield return WarningAction(attackManager);
            mob.Invincible = true;

            yield return new WaitForSeconds(0.5f);
            mob.PlayEffect(BaseTrembleEffectName, transform.position);
            yield return new WaitForSeconds(AttackInterval);

            for (int i = 0; i < AttackCount; ++i)
            {
                var randomX = Random.Range(SpawnRange.bounds.min.x, SpawnRange.bounds.max.x);
                var randomY = SpawnRange.bounds.min.y + Random.Range(1f, 2f);
                var randomPos = new Vector2(randomX, randomY);
                _objs.Add(Instantiate(TargetIndicator, randomPos, TargetIndicator.transform.rotation));
                yield return new WaitForSeconds(TargetSpawnDelay);
                mob.PlayEffect(TrembleEffectName, randomPos);
                mob.PlayAudio("Skill3");
                _objs.Add(Instantiate(FireBlast, randomPos, FireBlast.transform.rotation));
                yield return new WaitForSeconds(AttackInterval);
            }

            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(false);
            mob.Invincible = false;
        }
    }
}
