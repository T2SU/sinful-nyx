using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sevens.Entities.Players;
using System.IO;
using Sevens.Scenes;
using System.Text;
using System.Security.Cryptography;
using Sevens.Speeches;
using Sevens.Utils;
using System;
using static System.Environment;
using UnityEngine.UI;
using Sevens.Cameras;

public class SceneManagement : MonoBehaviour
{
    private static SceneManagement instance;
    public static SceneManagement Instance {
        get {
            if(!instance) {
                instance = new GameObject("SceneManagement", typeof(SceneManagement)).GetComponent<SceneManagement>();
                /*instance = FindObjectOfType(typeof(SceneManagement)) as SceneManagement;

                if(instance == null) {
                    Debug.Log("Can't find the SceneManagement Object");
                }*/
            }
            return instance;
        }
    }

    public class PlayerData {
        public string spawnPointName;
        public float playerHP = 100f;
        public float playerHPRatio;
        public float playerSin = 0f;
        public string savedScene;
        public string[] datas = new string[(int)PlayerDataKeyType.Number];

        public void SetData(PlayerDataKeyType type, string value)
        {
            datas[(int)type] = value;
        }

        public string GetData(PlayerDataKeyType type)
            => datas[(int)type];
    }
    public bool shouldLoadFromJson;
    public bool sceneIsLoadedBySaved;

    private string fileName = "Save Data";
    private string path;
    private Player playerInfo;

    private PlayerData playerData = new PlayerData();
    private Encoding encoding;

    [SerializeField]
    private Image _hpBar;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else if(instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        var document = Environment.GetFolderPath(SpecialFolder.MyDocuments);
        var dir = Path.Combine(document, "SevenChallengers");
        try
        {
            Directory.CreateDirectory(dir);
        } catch { }
        path = Path.Combine(dir, "SaveData.bin");
        encoding = Encoding.UTF8;

        //SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        LoadFromJsonFile();
    }

    public void ResetPlayerData()
    {
        playerData = new PlayerData();
    }

    public void LoadSceneByName(string sceneName)
    {
        shouldLoadFromJson = false;
        RetrievePlayerData();
        SceneManager.LoadScene(sceneName);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        sceneIsLoadedBySaved = shouldLoadFromJson;
        if (shouldLoadFromJson) {
            LoadFromJsonFile();
            shouldLoadFromJson = false;
        }
        else {
            Debug.Log($"Fired scene loaded");
            playerInfo = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
            if (playerInfo == null)
                return;
            ApplyPlayerData();
        }

        var spawnPoint = GameObject.Find("SpawnPoint").GetComponent<Transform>();

        playerInfo.transform.position = spawnPoint.position;

        var cameraBound = GameObject.Find("CM vcam1").GetComponent<VirtualCameraReferenceAutoFinder>();
        cameraBound.ExecuteReferComponents();
    }

    public void SetPlayerData(PlayerDataKeyType key, string value)
        => playerData.SetData(key, value);

    public string GetPlayerData(PlayerDataKeyType key)
        => playerData.GetData(key);

    public void SaveToJsonFile()
    {
        RetrievePlayerData();

        string json = JsonUtility.ToJson(playerData);
        Debug.Log(playerData.playerHP);
        var binary = encoding.GetBytes(json);
        using var rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.ECB;
        rijndael.Padding = PaddingMode.Zeros;
        rijndael.KeySize = 128;
        rijndael.BlockSize = 128;
        rijndael.Key = encoding.GetBytes("52V2NCh@11eng2Rs");
        var encryptor = rijndael.CreateEncryptor();
        binary = encryptor.TransformFinalBlock(binary, 0, binary.Length);
        File.WriteAllBytes(path, binary);
    }

    private void RetrievePlayerData()
    {
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerData.playerHP = playerInfo.Hp;
        playerData.playerSin = playerInfo.Sin;
        playerData.spawnPointName = playerInfo.GetClosestSpawnPoint();
        playerData.savedScene = SceneManager.GetActiveScene().name;
    }

    private void ApplyPlayerData()
    {
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerInfo.SetInitialHp(playerData.playerHP);
        playerInfo.Sin = playerData.playerSin;
        if (SceneManager.GetActiveScene().name != playerData.savedScene)
            return;
        if (!string.IsNullOrEmpty(playerData.spawnPointName))
        {
            var pointObj = GameObject.Find(playerData.spawnPointName);
            if (pointObj == null)
                return;
            var pos = pointObj.transform.position;
            playerInfo.transform.position = pos;
        }
    }

    public void LoadFromJsonFile() {
        var binary = File.ReadAllBytes(path);
        using var rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.ECB;
        rijndael.Padding = PaddingMode.Zeros;
        rijndael.KeySize = 128;
        rijndael.BlockSize = 128;
        rijndael.Key = encoding.GetBytes("52V2NCh@11eng2Rs");
        var decryptor = rijndael.CreateDecryptor();
        binary = decryptor.TransformFinalBlock(binary, 0, binary.Length);
        string jsonLoaded = Encoding.UTF8.GetString(binary);
        playerData = JsonUtility.FromJson<PlayerData>(jsonLoaded);

        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerInfo.SetInitialHp(playerData.playerHP);
        playerInfo.SetInitialSin(playerData.playerSin);
        if (!string.IsNullOrEmpty(playerData.spawnPointName))
        {
            var pointObj = GameObject.Find(playerData.spawnPointName);
            var pos = pointObj.transform.position;
            playerInfo.transform.position = pos;
        }
    }

    public string GetScene() {
        var binary = File.ReadAllBytes(path);
        using var rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.ECB;
        rijndael.Padding = PaddingMode.Zeros;
        rijndael.KeySize = 128;
        rijndael.BlockSize = 128;
        rijndael.Key = encoding.GetBytes("52V2NCh@11eng2Rs");
        var decryptor = rijndael.CreateDecryptor();
        binary = decryptor.TransformFinalBlock(binary, 0, binary.Length);
        string jsonLoaded = Encoding.UTF8.GetString(binary);
        PlayerData playerDataLoaded = JsonUtility.FromJson<PlayerData>(jsonLoaded);

        return playerDataLoaded.savedScene;
    }
}
