using UnityEngine;

namespace Assets.Scripts.Sevens.Scenes
{
    public class SaveGameOnLoad : MonoBehaviour
    {
        private void OnEnable()
        {
            _SceneManagement.Instance.SaveToJsonFile();
        }
    }
}
