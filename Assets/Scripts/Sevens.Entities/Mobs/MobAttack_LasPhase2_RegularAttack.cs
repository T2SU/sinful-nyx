using DG.Tweening;
using Sevens.Entities.Players;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_RegularAttack : MobAttackBase
    {
        public GameObject HandLeft;
        public GameObject HandRight;
        public Transform HandLeftInitialPosition;
        public Transform HandRightInitialPosition;

        public float HandMoveDuration;
        public float HandSmashDownDuration;
        public float HandSweepAttackDuration;
        public float HandWaitForUpDuration;
        public float AttackInterval;
        public float AttackTimeScale;
        public string SmashEffectName;
        public Transform GroundPosition;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase2_RegularAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            mob.ClearSpineAnimations(0, 0, 1);
            attackManager.EndAttack(true);
            ClearObjects();
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;

            mob.PlayAnimation(
                new AnimationPlayOption("IdleNoHand", track: 1, loop: true, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );

            var leftHand = Instantiate(HandLeft, transform);
            var rightHand = Instantiate(HandRight, transform);
            _objs.Add(leftHand);
            _objs.Add(rightHand);
            SetAllBlowSourceAs(leftHand, mob);
            SetAllBlowSourceAs(rightHand, mob);
            var leftReturnPos = leftHand.transform.position;
            var rightReturnPos = rightHand.transform.position;
            var leftReturnRot = leftHand.transform.rotation;
            var rightReturnRot = rightHand.transform.rotation;
            coroutines.Register("LeftHandInitialMove", leftHand.transform.DOMove(HandLeftInitialPosition.transform.position, 0.5f));
            coroutines.Register("RightHandInitialMove", rightHand.transform.DOMove(HandRightInitialPosition.transform.position, 0.5f));
            coroutines.Register("LeftHandInitialRotate", leftHand.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.5f));
            coroutines.Register("RightHandInitialRotate", rightHand.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.5f));
            leftHand.GetComponent<Blow>().OnceOption.Enabled = false;
            rightHand.GetComponent<Blow>().OnceOption.Enabled = false;
            yield return new WaitForSeconds(0.7f);


            CustomYieldInstruction DoAttack(GameObject hand, Transform initPos, Transform otherInitPos)
            {
                var y = hand.transform.position.y;
                var handBlow = hand.GetComponent<Blow>();
                var targetY = GroundPosition.position.y + hand.GetComponent<Collider2D>().bounds.size.y / 3;
                var isSequenceRunning = true;
                var beginPos = initPos.position;
                var seq = DOTween.Sequence()
                    .Append(hand.transform.DOMoveY(targetY, HandSmashDownDuration))
                    .AppendCallback(() => mob.PlayEffect(SmashEffectName, hand.transform.position))
                    .AppendCallback(() => handBlow.OnceOption.Enabled = true)
                    .Append(hand.transform.DOMoveX(otherInitPos.position.x, HandSweepAttackDuration))
                    .AppendInterval(HandWaitForUpDuration)
                    .AppendCallback(() => handBlow.OnceOption.Enabled = false)
                    .Append(hand.transform.DOMove(beginPos, HandMoveDuration))
                    .AppendInterval(AttackInterval)
                    .OnComplete(() => isSequenceRunning = false);
                coroutines.Register("FistAttack", seq);
                return new WaitWhile(() => isSequenceRunning);
            }

            var patternType = Random.Range(0, 2);
            if (patternType == 0)
            {
                yield return WarningAction(attackManager);
                yield return DoAttack(leftHand, HandLeftInitialPosition, HandRightInitialPosition);
                yield return WarningAction(attackManager);
                yield return DoAttack(rightHand, HandRightInitialPosition, HandLeftInitialPosition);
                yield return WarningAction(attackManager);
                yield return DoAttack(leftHand, HandLeftInitialPosition, HandRightInitialPosition);
            }
            else
            {
                yield return WarningAction(attackManager);
                yield return DoAttack(rightHand, HandRightInitialPosition, HandLeftInitialPosition);
                yield return WarningAction(attackManager);
                yield return DoAttack(leftHand, HandLeftInitialPosition, HandRightInitialPosition);
                yield return WarningAction(attackManager);
                yield return DoAttack(rightHand, HandRightInitialPosition, HandLeftInitialPosition);
            }

            coroutines.Register("LeftFistFinalMove", leftHand.transform.DOMove(leftReturnPos, 0.5f));
            coroutines.Register("RightFistFinalMove", rightHand.transform.DOMove(rightReturnPos, 0.5f));
            coroutines.Register("LeftFistFinalRotate", leftHand.transform.DOLocalRotateQuaternion(leftReturnRot, 0.5f));
            coroutines.Register("RightFistFinalRotate", rightHand.transform.DOLocalRotateQuaternion(rightReturnRot, 0.5f));
            yield return new WaitForSeconds(0.7f);

            mob.ClearSpineAnimations(0, 0, 1);
            attackManager.EndAttack(false);
            ClearObjects();
        }
    }
}
