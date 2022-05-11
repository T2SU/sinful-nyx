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
        private float _guardCooltimer;
        private bool _isParry;

        private bool _isGuard;


        // 받을 데미지랑 깎일 스태미너 양?
        public PlayerGuardResult TryGuard(Entity source, float damage) 
        {
            var result = new PlayerGuardResult(damage);
            
            if (CheckGuardDirection(source))
            {
                if (_isParry)
                {
                    result.Damage = 0f;
                    result.StaminaDamage = 0f;
                    result.Guarded |= PlayerGuardResultType.Parry | PlayerGuardResultType.Guard;
                    _guardCooltimer = 0f;
                }
                else if (_isGuard)
                {
                    result.Damage = damage * _guardInfoComponent.ReduceDamageRatio;
                    result.StaminaDamage = damage * _guardInfoComponent.StaminaDamageRatio;
                    result.Guarded = PlayerGuardResultType.Guard;
                }
            }
            return result;
        }

        private void Update()
        {
            _guardCooltimer += Time.deltaTime;

            if (_playerComponent.State != PlayerState.Idle && _playerComponent.State != PlayerState.Run && _playerComponent.State != PlayerState.Guard) return;

            if (_isParry)
            {
                _parryableTimer += Time.deltaTime;
                if (_parryableTimer >= _guardInfoComponent.parryableTime)
                {
                    _isParry = false;
                }
            }

            if (_guardCooltimer > _guardInfoComponent.guardCooltime)
            {
                if (Input.GetButtonDown("Guard"))
                {
                    _isParry = true;
                    _guardCooltimer = 0f;
                }

                if (Input.GetButton("Guard"))
                {
                    _isGuard = true;
                    _playerComponent.State = PlayerState.Guard;
                }
                else
                {
                    _isGuard = false;
                    if(!_isParry) _playerComponent.State = PlayerState.Idle;
                }
            }
        }

        private bool CheckGuardDirection(Entity source)
        {
            return _playerComponent.InOnLeftBy(source.transform) == _playerComponent.IsFacingLeft();
        }
    }

    public struct PlayerGuardResult
    {
        public float Damage;
        public float StaminaDamage;
        public PlayerGuardResultType Guarded;

        public PlayerGuardResult(float damage)
        {
            Damage = damage;
            StaminaDamage = 0f;
            Guarded = 0;
        }
    }
}