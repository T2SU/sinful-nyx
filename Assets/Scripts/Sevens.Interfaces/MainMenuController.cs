using UnityEngine;

namespace Sevens.Interfaces
{
    public class MainMenuController : MonoBehaviour
    {
        public string FirstSceneName;

        public void OnClickContinue()
        {
            // Continue Game
        }

        public void OnClickNewGame()
        {
            _SceneManagement.Instance.LoadSceneByName(FirstSceneName);
        }

        public void OnClickExit()
        {
            Application.Quit();
        }
    }
}
