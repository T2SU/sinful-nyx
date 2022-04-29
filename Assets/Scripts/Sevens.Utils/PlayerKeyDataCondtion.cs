using System;
using System.Linq;
using UnityEngine;

namespace Sevens.Utils
{

    [Serializable]
    public class PlayerKeyDataCondtionEntry
    {
        public enum EqualType
        {
            Equal,
            NotEqual
        }

        public PlayerDataKeyType Type;
        public EqualType Equal;
        public string Value;

        public bool IsSatisfied()
        {
            if (Equal == EqualType.NotEqual)
            {
                return SceneManagement.Instance.GetPlayerData(Type) != Value;
            }
            else
            {
                return SceneManagement.Instance.GetPlayerData(Type) == Value;
            }
        }
    }

    public class PlayerKeyDataCondtion : MonoBehaviour
    {
        public enum ConnectType
        {
            Or,
            And
        }

        public PlayerKeyDataCondtionEntry[] Conditions;
        public ConnectType Type;

        private void Start()
        {
            bool conditionSatisfied;

            if (Type == ConnectType.Or)
            {
                conditionSatisfied = Conditions.Any(c => c.IsSatisfied());
            }
            else
            {
                conditionSatisfied = Conditions.All(c => c.IsSatisfied());
            }
            if (conditionSatisfied)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
    }
}
