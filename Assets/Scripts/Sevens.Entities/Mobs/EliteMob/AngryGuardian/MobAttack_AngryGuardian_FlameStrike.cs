using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.PlayAnimation(
                new AnimationPlayOption("Idle", timeScale: _attackTimeScale),
                immediatelyTransition: true);

            yield return new WaitForSeconds(_chargeTime);
            Instantiate(_flameStrike, _AttackPos1);
            mob.PlayAudio("FlameStrike");
            yield return new WaitForSeconds(_flameStrikeDelay);
            Instantiate(_flameStrike, _AttackPos2);
            mob.PlayAudio("FlameStrike");
            yield return new WaitForSeconds(_flameStrikeDelay);
            Instantiate(_flameStrike, _AttackPos3);
            mob.PlayAudio("FlameStrike");
        }
    }
}

