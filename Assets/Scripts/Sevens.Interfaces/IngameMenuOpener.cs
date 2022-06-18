using Sevens.Speeches;
using Sevens.Utils;
using UnityEngine;

namespace Sevens.Interfaces
{
    public class IngameMenuOpener : MonoBehaviour
    {
        private GameObject _ingameMenuPrefab;

        private GameObject _ingameMenu;

        public string ButtonName;

        private void Awake()
        {
            _ingameMenuPrefab = Resources.Load<GameObject>(Prefabs.IngameMenu);
        }

        private void Update()
        {
            if (DialogueManager.Instance.HasDialogue())
                return;
            if (!SceneManagement.Instance.IngameMenuAvailable)
                return;
            if (Input.GetButtonDown(ButtonName))
            {
                if (!EnsureIngameMenu())
                    return;
                var uiScroller = _ingameMenu.transform.GetComponentInChildren<UIScroller>();
                if (uiScroller.Displayed)
                    return;
                uiScroller.PlayForward();
            }
        }

        private bool EnsureIngameMenu()
        {
            if (!_ingameMenu)
            {
                _ingameMenu = Instantiate(_ingameMenuPrefab);
                return true;
            }
            return false;
        }
    }
}
