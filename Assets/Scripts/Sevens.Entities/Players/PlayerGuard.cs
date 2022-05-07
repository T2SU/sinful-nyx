using UnityEngine;
using System;
using System.Collections.Generic;

namespace Sevens.Entities.Players
{
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
                result.Guarded = true;
            }
            else if (_isGuard)
            {
                result.Damage = damage * _guardInfoComponent.ReduceDamageRatio;
                result.StaminaDamage = damage * _guardInfoComponent.StaminaDamageRatio;
                result.Guarded = true;
            }
            else
            {
                result.Damage = damage;
                result.StaminaDamage = 0f;
                result.Guarded = false;
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
        public bool Guarded;
    }
}