using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_SmashDownAttack : MobAttackBase
    {
        public GameObject FistLeft;
        public GameObject FistRight;
        public Transform FistLeftInitialPosition;
        public Transform FistRightInitialPosition;
        public Transform GroundPosition;

        public float FistMoveDuration;
        public float FistSmashDownDuration;
        public float FistWaitForUpDuration;
        public int AttackCount;
        public float AttackInterval;
        public float AttackTimeScale;
        public string SmashWarningEffectName;
        public string SmashEffectName;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.PlayAnimation(
                new AnimationPlayOption("IdleNoHand", track: 1, loop: true, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );

            var leftFist = Instantiate(FistLeft, transform);
            var rightFist = Instantiate(FistRight, transform);
            var leftReturnPos = leftFist.transform.position;
            var rightReturnPos = rightFist.transform.position;
            leftFist.GetComponent<Blow>().OnceOption.Enabled = false;
            rightFist.GetComponent<Blow>().OnceOption.Enabled = false;
            coroutines.Register("LeftFistInitialMove", leftFist.transform.DOMove(FistLeftInitialPosition.transform.position, 0.5f));
            coroutines.Register("RightFistInitialMove", rightFist.transform.DOMove(FistRightInitialPosition.transform.position, 0.5f));
            yield return new WaitForSeconds(0.7f);

            for (int i = 0; i < AttackCount; ++i)
            {
                yield return WarningAction(mob);
                var fist = player.IsOnLeftBy(transform) ? leftFist : rightFist;
                var fistBlow = fist.GetComponent<Blow>();
                var y = fist.transform.position.y;
                var playerPos = player.transform.position;
                playerPos.y = GroundPosition.position.y;
                var targetPos = playerPos;
                targetPos.y += fist.GetComponent<Collider2D>().bounds.size.y / 3;
                var isSequenceRunning = true;
                var seq = DOTween.Sequence()
                    .AppendCallback(() => mob.PlayEffect(SmashWarningEffectName, playerPos))
                    .Append(fist.transform.DOMoveX(targetPos.x, FistMoveDuration))
                    .AppendCallback(() => fistBlow.OnceOption.Enabled = true)
                    .Append(fist.transform.DOMoveY(targetPos.y, FistSmashDownDuration))
                    .AppendCallback(() => mob.PlayEffect(SmashEffectName, playerPos))
                    .AppendCallback(() => fistBlow.OnceOption.Enabled = false)
                    .AppendInterval(FistWaitForUpDuration)
                    .Append(fist.transform.DOMoveY(y, FistSmashDownDuration))
                    .OnComplete(() => isSequenceRunning = false);
                coroutines.Register("FistAttack", seq);
                yield return new WaitWhile(() => isSequenceRunning);
            }

            coroutines.Register("LeftFistFinalMove", leftFist.transform.DOMove(leftReturnPos, 0.5f));
            coroutines.Register("RightFistFinalMove", rightFist.transform.DOMove(rightReturnPos, 0.5f));
            yield return new WaitForSeconds(0.7f);
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
        }
    }
}
