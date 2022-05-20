using UnityEngine;

namespace Scripts
{
    public class DestroyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _essentialObjects;

        private void Awake()
        {
            foreach(var obj in _essentialObjects)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }
}
