using Sevens.Utils;
using UnityEngine;

namespace Sevens.Interfaces
{
    public class MainMenuController : MonoBehaviour
    {
        public string FirstSceneName;

        public void OnClickContinue()
        {
            SceneManagement.Instance.LoadGame();
        }

        public void OnClickNewGame()
        {
            SceneManagement.Instance.LoadScene(FirstSceneName);
        }

        public void OnClickExit()
        {
            Application.Quit();
        }
    }
}
