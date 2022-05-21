using Sevens.Entities.Players;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sevens.Utils
{
    public class SceneManagement : MonoBehaviour
    {
        private static SceneManagement _instance;
        public static SceneManagement Instance
        {
            get
            {
                if (!_instance)
                {
                    var obj = new GameObject("SceneManagement", typeof(SceneManagement));
                    _instance = obj.GetComponent<SceneManagement>();
                }
                return _instance;
            }
        }

        // 현재 씬이 이어하기로 로드 되었는지?
        public bool SceneIsLoadedBySaved;

        public void NewGame(string sceneName)
        {
            Singleton<PlayerData>.Data = null;
            SceneManager.LoadScene(sceneName);
        }

        public void LoadGame()
        {
            var data = SaveManager.LoadFromFile();
            Singleton<PlayerData>.Data = data;
            SceneIsLoadedBySaved = true;
            SceneManager.LoadScene(data.SceneName);
        }

        public void LoadScene(string sceneName)
        {
            // 임시 저장 파일
            var playerObj = GameObject.FindGameObjectWithTag("Player");

            // 씬에 플레이어가 있으면
            if (playerObj != null)
            {
                var player = playerObj.GetComponent<Player>();

                // 플레이어의 현재 상태를 싱글톤에 임시 저장
                Singleton<PlayerData>.Data = SaveManager.Serialize(player, sceneName, "SpawnPoint");
            }
            else
                Singleton<PlayerData>.Data = null;

            SceneIsLoadedBySaved = false;
            SceneManager.LoadScene(sceneName);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var data = Singleton<PlayerData>.Data;
            if (data != null)
                LoadPlayer(data);
        }

        private void LoadPlayer(PlayerData playerData)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            // 씬에 플레이어가 있으면
            if (playerObj != null)
            {
                // 싱글톤에서 가져온 플레이어 데이터를 적용
                var player = playerObj.GetComponent<Player>();

                player.Load(playerData);
            }
        }
    }
}
