using DG.Tweening;
using Sevens.Effects;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase1_EvasionWarp : MobAttackBase
    {
        public ActionEffectOption Effect;
        public float InvincibleTime;

        public Collider2D PlaceRange;

        public override string Name => "Evasion";

        private ContactFilter2D _filter;

        public override bool IsAvailable(Player player, Mob mob)
        {
            if (!base.IsAvailable(player, mob))
                return false;
            var list = new List<Collider2D>();
            if (Physics2D.OverlapCollider(PlaceRange, _filter, list) > 0)
                return false;
            var groundHit = Physics2D.Raycast(
                new Vector2(PlaceRange.bounds.center.x, PlaceRange.bounds.min.y),
                Vector2.down,
                1.0f,
                PhysicsUtils.GroundLayerMask);
            if (!groundHit)
                return false;
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            _filter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = PhysicsUtils.GroundLayerMask
            };
        }

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.SetInvincible(InvincibleTime);
            var pos = mob.transform.position;
            var dest = new Vector3(PlaceRange.bounds.center.x, pos.y, pos.z);
            Effect.Apply(mob.transform, pos, new Vector2(0, mob.EntitySizeY));
            mob.PlayAudio("Evasion");
            var seq = DOTween.Sequence()
                .Append(DOTween.To(() => mob.GetSkelAlpha(), a => mob.SetSkelAlpha(a), 0f, 0.15f))
                .AppendCallback(() =>
                {
                    coroutines.KillTweener("BounceBack");
                    mob.transform.position = dest;
                    mob.transform.SetFacingLeft(!mob.transform.IsFacingLeft());
                })
                .Append(DOTween.To(() => mob.GetSkelAlpha(), a => mob.SetSkelAlpha(a), 1f, 0.15f))
                .AppendInterval(0.25f);
            yield return seq.WaitForCompletion();
        }
    }
}
