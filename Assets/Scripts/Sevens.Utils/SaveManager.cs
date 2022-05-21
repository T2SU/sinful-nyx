using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sevens.Entities.Players;
using System;
using System.IO;
using static System.Environment;
using System.Security.Cryptography;

namespace Sevens.Utils
{
    public static class SaveManager
    {
        // SaveManager
        // 1. 세이브 & 로드

        // 임시 파일 -> memory 변수로 저장하면 되지 않나..

        private static Encoding _encoding = Encoding.UTF8;
        private static string _path;
        private static ICryptoTransform _encryptor;
        private static ICryptoTransform _decryptor;

        static SaveManager()
        {
            MakeSavePath("SaveData");
            MakeCrypto();
        }

        private static void MakeSavePath(string path)
        {
            var document = Environment.GetFolderPath(SpecialFolder.MyDocuments);
            var dir = Path.Combine(document, "SevenChallengers");
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch { }
            _path = Path.Combine(dir, $"{path}.bin");
        }

        private static void MakeCrypto()
        {
            var rijndael = new RijndaelManaged();
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.Zeros;
            rijndael.KeySize = 128;
            rijndael.BlockSize = 128;
            rijndael.Key = _encoding.GetBytes("52V2NCh@11eng2Rs");
            _encryptor = rijndael.CreateEncryptor();
            _decryptor = rijndael.CreateDecryptor();
        }

        public static void SaveGame()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
                return;
            var player = playerObj.GetComponent<Player>();
            var data = Serialize(player);
            SaveToFile(data);
        }

        public static PlayerData Serialize(Player player, string scene = null, string spawnPointName = null)
        {
            var ret = new PlayerData()
            {
                HP = player.Hp,
                MaxHP = player.MaxHp,
                Sin = player.Sin,
                MaxSin = player.MaxSin,
                Stamina = player.Stamina,
                MaxStamina = player.MaxStamina,
                Soul = player.Soul,
                Achievements = player.Achievements.Copy()
            };
            if (scene == null)
                ret.SceneName = SceneManager.GetActiveScene().name;
            if (spawnPointName == null)
                ret.SpawnPointName = player.GetClosestSpawnPoint();
            return ret;
        }

        public static PlayerData LoadFromFile()
        {
            // 파일에서 읽기
            var binary = File.ReadAllBytes(_path);
            //binary = _decryptor.TransformFinalBlock(binary, 0, binary.Length);
            string jsonLoaded = Encoding.UTF8.GetString(binary);
            return JsonUtility.FromJson<PlayerData>(jsonLoaded);
        }

        public static void SaveToFile(PlayerData playerData)
        {
            // 파일로 저장
            string json = JsonUtility.ToJson(playerData);
            var binary = _encoding.GetBytes(json);
            //binary = _encryptor.TransformFinalBlock(binary, 0, binary.Length);
            File.WriteAllBytes(_path, binary);
        }
    }
}