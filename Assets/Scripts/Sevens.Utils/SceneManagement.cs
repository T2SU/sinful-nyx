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

        // 화면 전환 로딩
        private SceneLoading _sceneLoading;

        private CoroutineMan _coroutineMan;

        public bool IngameMenuAvailable
        {
            get
            {
                // 플레이어가 연출 모드 중에는 인게임 메뉴 호출 불가능
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    var player = playerObj.GetComponent<Player>();
                    if (player != null)
                    {
                        if (player.IsDirectionMode())
                            return false;
                    }
                }

                // 화면 로딩 중일때 인게임 메뉴 호출 불가능
                if (_sceneLoading != null)
                    return _sceneLoading.IsDone;
                return true;
            }
        }

        public void NewGame(string sceneName)
        {
            Singleton<PlayerData>.Data = null;
            LoadScene(sceneName);
        }

        public void LoadGame()
        {
            var data = SaveManager.LoadFromFile();
            Singleton<PlayerData>.Data = data;
            SceneIsLoadedBySaved = true;
            LoadScene(data.SceneName);
        }

        public void LoadScene(string sceneName)
        {
            if (_sceneLoading != null && !_sceneLoading.IsDone)
            {
                Debug.LogError($"씬이 이미 로딩중 상태에 있습니다. 로딩 요청된 씬 이름: {sceneName}");
                return;
            }
            if (!SceneIsLoadedBySaved)
                StashPlayerData(sceneName);
            SceneIsLoadedBySaved = false;
            _sceneLoading = new SceneLoading(sceneName, _coroutineMan);
        }

        private void StashPlayerData(string sceneName)
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
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _coroutineMan = new CoroutineMan(this);
        }
    }
}
