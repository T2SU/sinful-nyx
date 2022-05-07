using UnityEngine;
using System;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class GuardInfo
    {
        //����� ���׹̳��� �������� ����ϴ� ����
        public float StaminaDamageRatio;

        //�и� ��Ÿ��
        public float guardCooltime;

        //�и� ���ӽð�
        public float parryableTime;

        //����� ������ ���� ����
        public float ReduceDamageRatio;

        //�и��� �ñر� ������ ������
        public float IncreaseUltGaugeOnParrying; 
    }
}
