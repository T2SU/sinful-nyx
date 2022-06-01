using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase1_SlashWaveAttack : MobAttackBase
    {
        public GameObject SlashWaveAttack;
        public float AttackTimeScale;
        public float AttackMotionDuration;
        public float SpwanSlashWaveDelay;
        public float SlashWaveSpeed;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase1_SlashWaveAttack);
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
            mob.PlayAnimation(
                new AnimationPlayOption("Skill3", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            mob.PlayAudio("Skill3");
            yield return WarningAction(attackManager);
            yield return new WaitForSeconds(SpwanSlashWaveDelay);
            var pos = mob.transform.position;
            var left = mob.transform.IsFacingLeft();
            var delta = SlashWaveAttack.transform.position;
            delta.x = PhysicsUtils.GetXByDirection(delta.x, left);
            var obj = Instantiate(SlashWaveAttack, pos + delta, mob.transform.rotation);
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            ApplyExpansionAnimation(obj, attackManager, mob, left);
            MakeWaveMoveForward(obj, attackManager, left);
            yield return new WaitForSeconds(AttackMotionDuration);
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(false);
        }

        private void ApplyExpansionAnimation(GameObject obj, MobAttackable attackManager, Mob mob, bool left)
        {
            obj.transform.localScale = new Vector3(PhysicsUtils.GetXByDirection(.1f, left), .1f, 1f);
            obj.transform.SetFacingLeft(mob.transform.IsFacingLeft());
            attackManager.AttackCoroutines.Register("Skill3ExpansionX", obj.transform.DOScaleX(PhysicsUtils.GetXByDirection(1f, left), 0.2f));
            attackManager.AttackCoroutines.Register("Skill3ExpansionY", obj.transform.DOScaleY(1f, 0.2f));
        }

        private void MakeWaveMoveForward(GameObject obj, MobAttackable attackManager, bool left)
        {
            var rigid = obj.GetComponent<Rigidbody2D>();

            var velocity = Vector2.right * SlashWaveSpeed;
            velocity.x = PhysicsUtils.GetXByDirection(velocity.x, left);

            attackManager.AttackCoroutines.Register(
                "Skill3MoveForward",
                DOTween.To(() => rigid.velocity, v => rigid.velocity = v, velocity, 0.3f)
                .SetEase(Ease.Linear)
            );
        }
    }
}
