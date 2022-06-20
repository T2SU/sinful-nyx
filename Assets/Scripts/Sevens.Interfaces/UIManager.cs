using Sevens.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Sevens.Interfaces
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        private static GameObject _popUpPrefab;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("UIManager", typeof(UIManager));
                    _instance = obj.GetComponent<UIManager>();
                }
                return _instance;
            }
        }

        private Transform _popUp;

        public PopupWait Popup(string text, string selection1, string selection2, UnityAction action1 = null, UnityAction action2 = null)
        {
            if (_popUp != null)
                Destroy(_popUp.gameObject);
            _popUp = Instantiate(_popUpPrefab).transform;

            var popup = _popUp.GetComponent<Popup>();
            popup.SetText(text);
            popup.SetSelectionText(0, selection1);
            popup.SetSelectionText(1, selection2);
            if (action1 != null)
                popup.AddClickListener(0, action1);
            if (action2 != null)
                popup.AddClickListener(1, action2);

            return popup.Show();
        }

        private void Awake()
        {
            _popUpPrefab = Resources.Load<GameObject>(Prefabs.Popup);
        }
    }
}
