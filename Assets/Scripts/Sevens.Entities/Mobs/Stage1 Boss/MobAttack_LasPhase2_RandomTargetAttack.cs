using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
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

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.PlayAnimation(
                new AnimationPlayOption("Skill3", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            yield return WarningAction(mob);
            mob.Invincible = true;

            yield return new WaitForSeconds(0.5f);
            mob.PlayEffect(BaseTrembleEffectName, transform.position);
            yield return new WaitForSeconds(AttackInterval);

            for (int i = 0; i < AttackCount; ++i)
            {
                var randomX = Random.Range(SpawnRange.bounds.min.x, SpawnRange.bounds.max.x);
                var randomY = SpawnRange.bounds.min.y + Random.Range(1f, 2f);
                var randomPos = new Vector2(randomX, randomY);
                Instantiate(TargetIndicator, randomPos, TargetIndicator.transform.rotation);
                yield return new WaitForSeconds(TargetSpawnDelay);
                mob.PlayEffect(TrembleEffectName, randomPos);
                mob.PlayAudio("Skill3");
                Instantiate(FireBlast, randomPos, FireBlast.transform.rotation);
                yield return new WaitForSeconds(AttackInterval);
            }
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.Mob.Invincible = false;
        }
    }
}
