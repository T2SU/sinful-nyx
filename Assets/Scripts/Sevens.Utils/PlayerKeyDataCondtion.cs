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

        private void Awake()
        {
            SceneManager.sceneLoaded += Init;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= Init;
        }

        private void Init(Scene scene, LoadSceneMode mode)
        {
            var target = Target;
            if (target == null)
                target = gameObject;

            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                Debug.LogError("Cannot find 'Player' in this scene.");
                target.SetActive(false);
                return;
            }
            var player = playerObj.GetComponent<Player>();

            bool conditionSatisfied;

            if (Type == ConnectType.Or)
            {
                conditionSatisfied = Conditions.Any(c => c.IsSatisfied(player));
                Debug.Log(conditionSatisfied);
            }
            else
            {
                conditionSatisfied = Conditions.All(c => c.IsSatisfied(player));
            }
            if (conditionSatisfied)
                target.SetActive(true);
            else
                target.SetActive(false);
        }
    }
}
