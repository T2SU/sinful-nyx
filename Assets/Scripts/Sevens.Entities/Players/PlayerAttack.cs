using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField, Header("Combo Delay Time")]
        private float _comboDelay;
        private int _comboCount;
        private float _comboTimer;
        private float xDirection;

        // 플레이어 변수, 프로퍼티 및 함수 불러오기
        private Player _player;
        private AttackInfo _attackInfo;

        // 콤보 오브젝트
        [SerializeField, Header("Combo Objects Array")]
        GameObject[] _comboObjects;

        // 통상 공격
        public void NormalAttack()
        {
            if(_player.State == PlayerState.Die || _player.State == PlayerState.Dash)
            {
                return;
            }

            GameObject combo = null;
            Vector3 comboPosition = new Vector3(transform.position.x + (3f * xDirection), transform.position.y + 3.4f, transform.position.z);

            if(_comboCount == 0)
            {
                _player.ChangeState(PlayerState.Attack1);
                _player.PlayAudio("NyxAttack1");
                combo = Instantiate(_comboObjects[0], comboPosition, Quaternion.identity);
                _comboCount++;
                _comboTimer = 0;
            }
            else if(_comboCount == 1 && _comboTimer < _comboDelay)
            {
                _player.ChangeState(PlayerState.Attack2);
                _player.PlayAudio("NyxAttack2");
                combo = Instantiate(_comboObjects[1], comboPosition, Quaternion.identity);
                _comboCount++;
                _comboTimer = 0;
            }
            else if(_comboCount == 2 && _comboTimer < _comboDelay)
            {
                _player.ChangeState(PlayerState.Attack3);
                _player.PlayAudio("NyxAttack3");
                combo = Instantiate(_comboObjects[2], comboPosition, Quaternion.identity);
                _comboCount = 0;
                _comboTimer = -0.3f;
            }

            if(combo != null)
            {
                Blow.SetAllBlowSourceAs(combo, _player);
            }
        }

        // 공중 공격
        public void AirAttack()
        {

        }

        protected void Awake()
        {
            _player = GetComponent<Player>();
        }

        protected void Update()
        {
            if(Input.GetAxisRaw("Horizontal") > 0)
            {
                xDirection = 1f;
            }
            else if(Input.GetAxisRaw("Horizontal") < 0)
            {
                xDirection = -1f;
            }

            _comboTimer += Time.deltaTime;

            if(Input.GetButtonDown("Fire1") && _attackInfo.Cooltime < _comboTimer && _player.State == PlayerState.Die && _player.State == PlayerState.Dash)
            {
                if(!_player.IsDirectionMode) {
                    NormalAttack();
                }
            }
        }
    }
}