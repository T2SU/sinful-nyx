using System;
using UnityEngine;

namespace Sevens.Entities.Players
{
    [Serializable]
    public class AttackInfo
    {
        public GameObject AttackPrefab;
        public string AudioName;
        public string AnimationName;

        // 콤보1 ~ 콤보2, 콤보2 ~ 콤보3 사이에 기본적으로 적용되는 딜레이
        public float ComboDelay;

        // 콤보 공격이 모두 종료되었을 경우, 현재 AttackInfo의 Cooltime으로 재공격 쿨타임이 적용됨.
        // 콤보1, 콤보2, 콤보3 각각 AttackInfo를 따로 가짐.
        public float AttackCooltime;

        // 콤보 공격이 모두 종료되었을 경우,
        // 먼저 ComboDelay 만큼의 시간이 경과된 이후 추가적으로 StateChangeDelay가 지난 후에
        // 기존 State로 변경됨.
        public float ComboFinishDelay;
    }
}
