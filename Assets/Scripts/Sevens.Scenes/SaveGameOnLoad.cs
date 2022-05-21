using Sevens.Utils;
using UnityEngine;

namespace Sevens.Scenes
{
    public class SaveGameOnLoad : MonoBehaviour
    {
        private void OnEnable()
        {
            SaveManager.SaveGame();
        }
    }
}
