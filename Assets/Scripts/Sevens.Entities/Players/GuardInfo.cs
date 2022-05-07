using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class GuardInfo
    {
        //가드시 스테미나가 데미지를 흡수하는 비율
        public float StaminaDamageRatio;

        //패링 쿨타임
        public float guardCooltime;

        //패링 지속시간
        public float parryableTime;

        //가드시 데미지 감소 비율
        public float ReduceDamageRatio;

        //패링시 궁극기 게이지 증가량
        public float IncreaseUltGaugeOnParrying; 
    }
}
