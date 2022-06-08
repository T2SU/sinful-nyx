using Sevens.Entities.Players;
using Sevens.Utils;
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
        [SerializeField]
        private GameObject _newPivot;
        [SerializeField]
        private bool _isSetNewPivot;
        private Transform _playerTransform;

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
                if (_mob.IsDelayedByChangedState(0.3f))
                    return;
                _mob.ChangeState(MobState.Idle, playLoopAnimationByState: true);
            }

            // 이동 또는 대기 상태가 아닐 경우, 몬스터의 속력을 0으로 만듦.
            if (_mob.State != MobState.Move && _mob.State != MobState.Idle)
            {
                _mob.SetVelocity(Vector2.zero, linearly: false);
                return;
            }

            var mobtransform = (_newPivot) == null ? _mob.transform : _newPivot.transform;
            var mobPos = mobtransform.position;
            var mobIsOnLeft = _mob.IsOnLeftBy(_playerTransform);
            var playerPos = _playerTransform.position;
            var sign = Mathf.Sign(playerPos.x - mobPos.x);

            if (!_isSetNewPivot)
            {
                if (mobIsOnLeft == _mob.transform.IsFacingLeft())
                    _mob.transform.SetFacingLeft(!mobIsOnLeft);
            }
            else
            {
                mobIsOnLeft = mobPos.x < playerPos.x;

                if (mobIsOnLeft == mobtransform.IsFacingLeft())
                    _mob.transform.SetFacingLeft(mobIsOnLeft, _newPivot.transform);
                else
                    _mob.transform.SetFacingLeft(mobIsOnLeft, _newPivot.transform);

            }


            // 타입에 따른 몬스터 이동. Velocity를 조절할 수 있게 해줌.
            if (Mathf.Abs(mobPos.x - playerPos.x) >= 2f)
            {
                if (!_mob.IsVelocityChangingLinearly())
                {
                    if (_mob.State == MobState.Idle)
                    {
                        if (_mob.IsDelayedByChangedState(1.0f))
                            return;
                        _mob.ChangeState(MobState.Move, playLoopAnimationByState: true);
                        _mob.SetVelocity(new Vector2(sign * _mob.MoveSpeed, 0), linearly: true);
                    }
                }
            }
            else
            {
                if (!_mob.IsVelocityChangingLinearly())
                {
                    if (_mob.State == MobState.Move)
                    {
                        _mob.ChangeState(MobState.Idle, playLoopAnimationByState: true);
                        _mob.SetVelocity(Vector2.zero, linearly: true);
                    }
                }
            }

            // 보는 방향과 반대로 이동하려고 할 때, 이동 방향만 전환
            if (!_mob.IsVelocityChangingLinearly())
            {
                if (_mob.State == MobState.Move)
                {
                    var vel = _mob.GetVelocity();
                    if (Mathf.Sign(vel.x) != sign)
                    {
                        vel.x = -vel.x;
                        _mob.SetVelocity(vel, linearly: false);
                    }
                }
            }
        }
    }
}
