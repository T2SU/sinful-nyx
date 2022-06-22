using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_AngryGuardian_FlameStrike : MobAttackBase
    {
        [SerializeField]
        private GameObject _flameStrike;
        [SerializeField]
        private float _attackTimeScale;
        [SerializeField]
        private float _chargeTime;
        [SerializeField]
        private float _flameStrikeDelay;
        [SerializeField]
        private Transform _AttackPos1;
        [SerializeField]
        private Transform _AttackPos2;
        [SerializeField]
        private Transform _AttackPos3;
        [SerializeField]
        private VisualEffect _chargeEffect;
        [SerializeField]
        private GameObject _flameStrikeEffect;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            _chargeEffect.Play();
            mob.PlayAnimation(
                new AnimationPlayOption("Idle", timeScale: _attackTimeScale),
                immediatelyTransition: true);

            yield return new WaitForSeconds(_chargeTime);
            _chargeEffect.Stop();
            Instantiate(_flameStrike, _AttackPos1);
            GameObject obj1 = Instantiate(_flameStrikeEffect, _AttackPos1);
            Destroy(obj1, 2f);
            mob.PlayAudio("FlameStrike");
            yield return new WaitForSeconds(_flameStrikeDelay);
            Instantiate(_flameStrike, _AttackPos2);
            GameObject obj2 = Instantiate(_flameStrikeEffect, _AttackPos2);
            Destroy(obj2, 2f);
            mob.PlayAudio("FlameStrike");
            yield return new WaitForSeconds(_flameStrikeDelay);
            Instantiate(_flameStrike, _AttackPos3);
            GameObject obj3 = Instantiate(_flameStrikeEffect, _AttackPos3);
            Destroy(obj3, 2f);
            mob.PlayAudio("FlameStrike");
        }
    }
}

