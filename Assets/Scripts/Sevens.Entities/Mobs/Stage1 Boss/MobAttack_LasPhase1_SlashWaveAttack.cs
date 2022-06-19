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

        private void ApplyExpansionAnimation(GameObject obj, CoroutineMan coroutines, Mob mob, bool left)
        {
            obj.transform.localScale = new Vector3(PhysicsUtils.GetXByDirection(.1f, left), .1f, 1f);
            obj.transform.SetFacingLeft(mob.transform.IsFacingLeft());
            coroutines.Register("Skill3ExpansionX", obj.transform.DOScaleX(PhysicsUtils.GetXByDirection(1f, left), 0.2f));
            coroutines.Register("Skill3ExpansionY", obj.transform.DOScaleY(1f, 0.2f));
        }

        private void MakeWaveMoveForward(GameObject obj, CoroutineMan coroutines, bool left)
        {
            var rigid = obj.GetComponent<Rigidbody2D>();

            var velocity = Vector2.right * SlashWaveSpeed;
            velocity.x = PhysicsUtils.GetXByDirection(velocity.x, left);

            coroutines.Register(
                "Skill3MoveForward",
                DOTween.To(() => rigid.velocity, v => rigid.velocity = v, velocity, 0.3f)
                .SetEase(Ease.Linear)
            );
        }

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.PlayAnimation(
                new AnimationPlayOption("Skill3", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            mob.PlayAudio("Skill3");
            yield return WarningAction(mob);
            yield return new WaitForSeconds(SpwanSlashWaveDelay);
            var pos = mob.transform.position;
            var left = mob.transform.IsFacingLeft();
            var delta = SlashWaveAttack.transform.position;
            delta.x = PhysicsUtils.GetXByDirection(delta.x, left);
            var obj = Instantiate(SlashWaveAttack, pos + delta, mob.transform.rotation);
            ApplyExpansionAnimation(obj, coroutines, mob, left);
            MakeWaveMoveForward(obj, coroutines, left);
            yield return new WaitForSeconds(AttackMotionDuration);
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
        }
    }
}
