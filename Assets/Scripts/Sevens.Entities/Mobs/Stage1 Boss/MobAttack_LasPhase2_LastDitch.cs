using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_LastDitch : MobAttackBase
    {
        public GameObject[] Totems;
        public Transform HealEffectPosition;

        public float AttackTimeScale;
        public int AnimCount = 3;
        public float AnimShakeDelay = 0.1f;

        public int TotemCreationNumber;
        public float TotemCreationInterval;
        public float FinalLimitTime;
        public float RecoverHPRatio;
        public Collider2D SpawnRange;

        public string TrembleEffectName;

        private Vector2? PickPosition(LasPhase2_Totem totem)
        {
            var min = SpawnRange.bounds.min;
            var max = SpawnRange.bounds.max;
            var mask = LayerMask.GetMask("Mob");

            for (int i = 0; i < 100; ++i) // trying 100
            {
                var x = Random.Range(min.x, max.x);
                var y = Random.Range(min.y, max.y);
                var minPos = new Vector2(x - totem.EntitySizeX * 2, y - totem.EntitySizeY * 2);
                var maxPos = new Vector2(x + totem.EntitySizeX * 2, y + totem.EntitySizeY * 2);
                var colliders = Physics2D.OverlapAreaAll(minPos, maxPos, mask);
                foreach (var col in colliders)
                {
                    if (col.GetComponent<LasPhase2_Totem>() == null)
                        continue;
                    goto NEXT_TRY;
                }
                return new Vector2(x, y);
            NEXT_TRY:
                continue;
            }
            return null;
        }

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.PlayAudio("LastDitch");

            mob.Invincible = true;
            mob.ClearSpineAnimations(0.3f, 0.3f, 0);

            yield return new WaitForSeconds(0.25f);

            WarningAction(mob);

            float interval = 0.6f;

            for (int i = 0; i < AnimCount; ++i)
            {
                mob.PlayAnimation(
                           new AnimationPlayOption("LastDitch", track: 1 + i, timeScale: AttackTimeScale),
                           immediatelyTransition: true);
                yield return new WaitForSeconds(AnimShakeDelay);
                mob.PlayEffect(TrembleEffectName, transform.position);
                yield return new WaitForSeconds(interval - AnimShakeDelay);
            }

            yield return new WaitForSeconds(1.2f);

            for (int i = 0; i < TotemCreationNumber; ++i)
            {
                var totem = Randomizer.PickOneRand(Totems);
                mob.PlayAudio("SummonTotem");
                var obj = Instantiate(totem, SpawnRange.transform);
                var pos = PickPosition(totem.GetComponent<LasPhase2_Totem>());
                if (pos != null)
                    obj.transform.position = pos.Value;
                yield return new WaitForSeconds(TotemCreationInterval);
            }

            float endTime = Time.time + FinalLimitTime;

            int GetTotemCount()
                => SpawnRange.GetComponentsInChildren<LasPhase2_Totem>().Length;

            yield return new WaitUntil(() => endTime < Time.time || GetTotemCount() == 0);

            var remainingTotems = SpawnRange.GetComponentsInChildren<LasPhase2_Totem>();
            var totemCount = remainingTotems.Length;

            if (totemCount > 0)
            {
                mob.Heal(mob.MaxHp * RecoverHPRatio * totemCount);
                mob.PlayEffect("Heal", HealEffectPosition.position);
            }
            foreach (var totem in remainingTotems)
                totem.DestroyTotem();
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, Enumerable.Range(1, AnimCount).ToArray());
            attackManager.Mob.Invincible = false;
        }
    }
}
