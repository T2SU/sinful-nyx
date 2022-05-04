using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    // 좌우 이동, 점프, 대쉬, 이단점프, 추락
    public class PlayerMove : MonoBehaviour
    {
        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        private void Update()
        {
            var axis = Input.GetAxis("Horizontal");
            var jump = Input.GetButtonDown("Jump");
        }

        private void FixedUpdate()
        {
            
        }
    }
}