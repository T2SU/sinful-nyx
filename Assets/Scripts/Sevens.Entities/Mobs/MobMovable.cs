﻿using Sevens.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    [RequireComponent(typeof(Mob))]
    public class MobMovable : MonoBehaviour
    {
        private Mob _mob;

        [SerializeField]
        private MobMoveType _moveType;

        private Transform _playerTransform;

        [SerializeField]
        private float _distanceToPlayer = 3f;


        private void Awake()
        {
            _mob = GetComponent<Mob>();
        }

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            // 몬스터가 고정 타입일 경우 이동 안시킴.
            if (_moveType == MobMoveType.Stationary)
                return;

            if (_mob.State == MobState.Hit)
            {
                if (_mob.IsDelayedByChangedState(_mob.HitTime))
                    return;
                _mob.ChangeState(MobState.Idle, playLoopAnimationByState: true);
            }

            // 타입에 따른 몬스터 이동. Velocity를 조절할 수 있게 해줌.
            var playerPos = _playerTransform.position;
            var mobPos = _mob.transform.position;
            var sign = Mathf.Sign(playerPos.x - mobPos.x);
            var mobIsOnLeft = _mob.IsOnLeftBy(_playerTransform);

            if (_mob.State == MobState.Idle || _mob.State == MobState.Move)
            {
                if (mobIsOnLeft == _mob.transform.IsFacingLeft())
                {
                    _mob.transform.SetFacingLeft(!mobIsOnLeft);
                    _mob.ChangeState(MobState.Idle, playLoopAnimationByState: true);
                    _mob.SetVelocity(Vector2.zero, linearly: true);

                }
            }

            if (Mathf.Abs(mobPos.x - playerPos.x) >= _distanceToPlayer)
            {
                if (_moveType == MobMoveType.ChasingPlayer)
                {
                    if (!_mob.IsVelocityChangingLinearly() && _mob.State == MobState.Idle)
                    {
                        //if (_mob.IsDelayedByChangedState(1.0f))
                        //    return;
                        _mob.ChangeState(MobState.Move, playLoopAnimationByState: true);
                        _mob.SetVelocity(new Vector2(sign * _mob.MoveSpeed, 0), linearly: true);
                    }
                }
            }
            else
            {
                if (!_mob.IsVelocityChangingLinearly() && _mob.State == MobState.Move)
                {
                    _mob.ChangeState(MobState.Idle, playLoopAnimationByState: true);
                    _mob.SetVelocity(Vector2.zero, linearly: true);
                }
            }
        }
    }
}
