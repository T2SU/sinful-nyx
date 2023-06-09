using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_AngryGuardian_StampAttack : MobAttackBase
    {
        [SerializeField]
        private GameObject _stampAttack;
        [SerializeField]
        private VisualEffect _stampAttackEffect;
        [SerializeField]
        private Transform _attackPos;
        [SerializeField]
        private float _attackTimeScale;
        [SerializeField]
        private float _attackDuration;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Attack", timeScale: _attackTimeScale),
                immediatelyTransition: true);
            yield return new WaitForSeconds(animTime - WarningDuration);
            yield return WarningAction(mob);
            Instantiate(_stampAttack, _attackPos.position, mob.transform.rotation);
            _stampAttackEffect.Play();
            mob.PlayAudio("StampAttack");
            yield return new WaitForSeconds(_attackDuration);
        }
    }

}
