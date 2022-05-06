using Sevens.Utils;
using UnityEngine;

namespace Sevens.Interfaces
{
    public class UIPanelExchanger : MonoBehaviour
    {
        public NamedObject<GameObject>[] Panels;
        public string DefaultPanel;

        private GameObject _currentPanel;

        /// <summary>
        /// 메뉴 패널 전환
        /// </summary>
        /// <param name="name"></param>
        public void SwitchPanel(string name)
        {
            GameObject panel = Panels.FindByName(name);
            if (panel == null)
            {
                Debug.LogWarning($"Not exists named panel: {name}");
                return;
            }
            if (_currentPanel)
                _currentPanel.SetActive(false);
            _currentPanel = panel;
            _currentPanel.SetActive(true);
        }

        private void Start()
        {
            if (DefaultPanel == null)
            {
                Debug.LogWarning($"Not defined default panel name");
                return;
            }
            SwitchPanel(DefaultPanel);
        }
    }
}
