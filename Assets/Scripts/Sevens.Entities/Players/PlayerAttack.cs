using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    public class PlayerAttack : MonoBehaviour
    {
        private int _comboCount;
        private float _comboDelay;
        private float _comboDamage;
        private float xDirection;

        // 플레이어 변수, 프로퍼티 및 함수 불러오기
        private Player player;

        // 콤보 오브젝트
        [SerializeField]
        GameObject[] _comboObjects;

        // 통상 공격
        public void NormalAttack() {
            if(player.State == PlayerState.Die || player.State == PlayerState.Dash) {
                return;
            }


        }

        // 공중 공격
        public void AirAttack() {

        }

        // (콤보 공격 가능 시)콤보 생성
        public void CreateCombo() {

        }

        private void Awake() {
            player = GetComponent<Player>();
        }

        private void Update() {
            if(Input.GetAxisRaw("Horizontal") > 0) {
                xDirection = 1f;
            }
            else if(Input.GetAxisRaw("Horizontal") < 0) {
                xDirection = -1f;
            }
        }
    }
}