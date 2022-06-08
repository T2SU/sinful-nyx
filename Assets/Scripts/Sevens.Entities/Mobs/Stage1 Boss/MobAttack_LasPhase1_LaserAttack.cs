using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase1_LaserAttack : MobAttackBase
    {
        [Serializable]
        public class LaserAngle
        {
            public float StartAngle;
            public float EndAngle;
        }

        public GameObject LaserGuide;
        public GameObject Laser;
        public float AttackTimeScale;

        public LaserAngle[] Angles;
        public float LaserRotationDuration;
        public AnimationCurve LaserEase;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase1_LaserAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(true);
            ClearObjects();
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Ult", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(attackManager);
            mob.PlayAudio("LaserAttack");
            var obj = Instantiate(Laser, LaserGuide.transform);
            _objs.Add(obj);
            SetAllBlowSourceAs(obj, mob);
            var laser = obj.GetComponent<Laser>();
            var firePoint = laser.FirePoint.transform;
            var angle = Randomizer.PickOneRand(Angles);
            firePoint.localRotation = Quaternion.AngleAxis(angle.StartAngle, Vector3.right);

            var endAngle = Quaternion.AngleAxis(angle.EndAngle, Vector3.right);
            coroutines.Register("LaserRotation", 
                DOTween.Sequence()
                .SetDelay(0.35f)
                .Append(firePoint.transform
                    .DOLocalRotateQuaternion(endAngle, LaserRotationDuration)
                    .SetEase(LaserEase))
                .AppendInterval(0.35f)
                .AppendCallback(() => {
                    mob.ClearSpineAnimations(0.3f, 0.3f, 1);
                    attackManager.EndAttack(false);
                    laser.DisableLaser();
                })
            );
        }
    }
}
