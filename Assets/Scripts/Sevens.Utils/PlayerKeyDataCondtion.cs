using Sevens.Entities.Players;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public bool IsSatisfied(Player player)
        {
            if (Equal == EqualType.NotEqual)
            {
                return player.Achievements.GetData(Type) != Value;
            }
            else
            {
                return player.Achievements.GetData(Type) == Value;
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

        public GameObject Target;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        

        public enum DebugType
        {
            None = 0,
            AlwaysDisable = 1,
            DisableIfEditor = 2,
            DisableIfDevelopmentBuild = 4
        }

        public DebugType DisableFlags;
#endif

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            var target = Target;
            if (target == null)
                target = gameObject;

            bool? conditionSatisfied = null;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (DisableFlags == DebugType.DisableIfEditor && Application.isEditor)
                conditionSatisfied = false;
            else if (DisableFlags == DebugType.DisableIfDevelopmentBuild && Debug.isDebugBuild)
                conditionSatisfied = false;
            else if (DisableFlags == DebugType.AlwaysDisable)
                conditionSatisfied = false;
#endif

            if (conditionSatisfied == null)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj == null)
                {
                    Debug.LogError("Cannot find 'Player' in this scene.");
                    target.SetActive(false);
                    return;
                }
                var player = playerObj.GetComponent<Player>();


                if (Type == ConnectType.Or)
                {
                    conditionSatisfied = Conditions.Any(c => c.IsSatisfied(player));
                    Debug.Log(conditionSatisfied);
                }
                else
                {
                    conditionSatisfied = Conditions.All(c => c.IsSatisfied(player));
                }
            }

            if (conditionSatisfied.Value)
                target.SetActive(true);
            else
                target.SetActive(false);
        }
    }
}
