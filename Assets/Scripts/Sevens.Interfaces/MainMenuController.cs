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
            // New Game
        }

        public void OnClickExit()
        {
            Application.Quit();
        }
    }
}
