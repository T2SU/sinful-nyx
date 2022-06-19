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

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
        }

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Ult", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(mob);
            mob.PlayAudio("LaserAttack");
            var obj = Instantiate(Laser, LaserGuide.transform);
            var laser = obj.GetComponent<Laser>();
            var firePoint = laser.FirePoint.transform;
            var angle = Randomizer.PickOneRand(Angles);
            firePoint.localRotation = Quaternion.AngleAxis(angle.StartAngle, Vector3.right);

            var endAngle = Quaternion.AngleAxis(angle.EndAngle, Vector3.right);
            yield return DOTween.Sequence()
                .SetDelay(0.35f)
                .Append(firePoint.transform
                    .DOLocalRotateQuaternion(endAngle, LaserRotationDuration)
                    .SetEase(LaserEase))
                .AppendInterval(0.35f).WaitForCompletion();
            laser.DisableLaser();
        }
    }
}
