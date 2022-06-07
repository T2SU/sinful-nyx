using DG.Tweening;
using Sevens.Effects;
using Sevens.Entities.Players;
using Sevens.Utils;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_LaserAttack : MobAttackBase
    {
        [Serializable]
        public class LaserAngle
        {
            public float StartAngle;
            public float EndAngle;
        }

        public GameObject LaserGuide;
        public GameObject LeftLaser;
        public GameObject RightLaser;
        public float AttackTimeScale;

        public LaserAngle[] LeftAngles;
        public LaserAngle[] RightAngles;
        public float LaserRotationDuration;
        public AnimationCurve LaserEase;

        public string LaserTrembleEffectName;

        private bool[] _isBeamActive;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase2_LaserAttack);
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
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Ult", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            yield return new WaitForSeconds(animTime - WarningDuration);
            mob.Invincible = true;
            yield return WarningAction(attackManager);
            yield return new WaitForSeconds(1.0f);
            var left = Instantiate(LeftLaser, LaserGuide.transform);
            var right = Instantiate(RightLaser, LaserGuide.transform);
            _objs.Add(left);
            _objs.Add(right);
            SetAllBlowSourceAs(left, mob);
            SetAllBlowSourceAs(right, mob);
            var leftLaser = left.GetComponent<MagicLaser>();
            var rightLaser = right.GetComponent<MagicLaser>();
            _isBeamActive = new[] { true, true };
            ControlLaser(attackManager, leftLaser, LeftAngles, 0);
            ControlLaser(attackManager, rightLaser, RightAngles, 1);
            mob.PlayAudio("LaserAttack");

            yield return new WaitForSeconds(0.3f);
            mob.PlayEffect(LaserTrembleEffectName, transform.position);

            yield return new WaitWhile(() => _isBeamActive.All(b => b));
            mob.PlayEffect(null, transform.position);
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(false);
            mob.Invincible = false;
        }

        private void ControlLaser(MobAttackable attackManager, MagicLaser laser, LaserAngle[] angles, int index)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;

            var firePoint = laser.FirePoint.transform;

            var angle = Randomizer.PickOneRand(angles);
            var baseAngle = Quaternion.AngleAxis(90f, Vector3.up);
            var beginAngle = baseAngle * Quaternion.AngleAxis(angle.StartAngle, Vector3.right);
            var endAngle = baseAngle * Quaternion.AngleAxis(angle.EndAngle, Vector3.right);

            firePoint.localRotation = beginAngle;

            coroutines.Register($"LaserMove_{laser.name}",
                DOTween.Sequence()
                .SetDelay(1.0f)
                .Append(firePoint
                    .DOLocalRotateQuaternion(endAngle, LaserRotationDuration)
                    .SetEase(LaserEase))
                .AppendInterval(0.4f)
                .AppendCallback(() => {
                    laser.DestroyLaser();
                    Debug.Log($"Destroyed {laser.name}");
                    _isBeamActive[index] = false;
                })
                .SetTarget(laser)
            );
        }
    }
}
