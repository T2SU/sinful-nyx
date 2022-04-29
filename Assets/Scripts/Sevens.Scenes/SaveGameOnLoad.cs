using UnityEngine;

namespace Assets.Scripts.Sevens.Scenes
{
    public class SaveGameOnLoad : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManagement.Instance.SaveToJsonFile();
        }
    }
}
