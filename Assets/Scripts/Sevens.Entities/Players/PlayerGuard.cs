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

        [SerializeField]
        private float _guardStamina;


        // 받을 데미지랑 깎일 스태미너 양?
        public PlayerGuardResult TryGuard(Entity source, float damage) 
        {
            var result = new PlayerGuardResult(damage);
            
            if (_playerComponent.CheckDirection(source))
            {
                if (_isParry)
                {
                    result.Damage = 0f;
                    result.StaminaDamage = 0f;
                    result.Guarded |= PlayerGuardResultType.Parry | PlayerGuardResultType.Guard;
                    _guardCooltimer = 0f; 
                    Debug.Log("패링성공");
                    _isGuard = false;
                    _isParry = false;
                }
                else if (_isGuard)
                {
                    result.Damage = damage * _guardInfoComponent.ReduceDamageRatio;
                    result.StaminaDamage = damage * _guardInfoComponent.StaminaDamageRatio;
                    result.Guarded = PlayerGuardResultType.Guard;
                    Debug.Log("가드성공");
                }
            }
            return result;
        }

        private void Update()
        {
            _guardCooltimer += Time.deltaTime;

            if (_playerComponent.State != PlayerState.Idle && _playerComponent.State != PlayerState.Run && _playerComponent.State != PlayerState.Guard ) return;
            if (_playerComponent.IsDirectionMode()) return;

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
                switch (_playerComponent.State)
                {
                    case PlayerState.Idle:
                    case PlayerState.Run:
                        if (Input.GetButtonDown("Guard") && _playerComponent.Stamina > 0)
                        {
                            _isParry = true;
                            _guardCooltimer = 0f;
                            _isGuard = true;
                            _playerComponent.State = PlayerState.Guard;
                            _playerComponent.Stamina -= _guardStamina;
                        }
                        break;
                    case PlayerState.Guard:
                        if (!Input.GetButton("Guard"))
                        {
                            _isGuard = false;
                            if (!_isParry) _playerComponent.State = PlayerState.Idle;
                        }
                        break;
                }
            }
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