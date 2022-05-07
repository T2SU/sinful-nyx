using UnityEngine;
using System;
using System.Collections.Generic;

namespace Sevens.Entities.Players
{
    public enum PlayerGuardResultType
    {
        Guard = 1 << 0,
        Parry = 1 << 1
    }

    public class PlayerGuard : MonoBehaviour 
    {
        [SerializeField]
        private GuardInfo _guardInfoComponent;
        [SerializeField]
        private Player _playerComponent;

        private float _parryableTimer;
        private float _parryCooltimer;
        private bool _isParry;

        private bool _isGuard;


        // 받을 데미지랑 깎일 스태미너 양?
        public PlayerGuardResult TryGuard(Entity source, float damage) 
        {
            var result = new PlayerGuardResult();
            if (_isParry)
            {
                result.Damage = 0f;
                result.StaminaDamage = 0f;
                result.Guarded |= PlayerGuardResultType.Parry | PlayerGuardResultType.Guard;
            }
            else if (_isGuard)
            {
                result.Damage = damage * _guardInfoComponent.ReduceDamageRatio;
                result.StaminaDamage = damage * _guardInfoComponent.StaminaDamageRatio;
                result.Guarded = PlayerGuardResultType.Guard;
            }
            else
            {
                result.Damage = damage;
                result.StaminaDamage = 0f;
            }
            return result;
        }

        private void Update()
        {
            _parryCooltimer += Time.deltaTime;

            if (_isParry)
            {
                _parryableTimer += Time.deltaTime;
                if(_parryableTimer >= _guardInfoComponent.parryableTime)
                {
                    _isParry = false;
                }
            }
            if (Input.GetButtonDown("Guard") && _parryCooltimer > _guardInfoComponent.parryCooltime)
            {
                _isParry = true;
                _parryCooltimer = 0f;
            }

            if (Input.GetButton("Guard"))
            {
                _isGuard = true;
            }
            else
            {
                _isGuard = false;
            }
        }
    }

    public struct PlayerGuardResult
    {
        public float Damage;
        public float StaminaDamage;
        public PlayerGuardResultType Guarded;
    }
}