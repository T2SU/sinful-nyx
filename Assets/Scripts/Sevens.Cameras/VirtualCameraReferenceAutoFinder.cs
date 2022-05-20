// The Seven deadly Sins
//
// Author  Seong Jun Mun (Tensiya(T2SU))
//         (liblugia@gmail.com)
//

using Cinemachine;
using System.Linq;
using UnityEngine;

namespace Sevens.Cameras
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class VirtualCameraReferenceAutoFinder : MonoBehaviour
    {
        private CinemachineVirtualCamera _vcam;
        private CinemachineConfiner _confiner;

        public string[] PlayerTags = new string[] { "Player" };
        public string[] CameraBoundNames = new string[] { "CameraBound", "Camera Bound" };

        private void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
            _confiner = GetComponent<CinemachineConfiner>();
        }

        private void OnValidate()
        {
            ExecuteReferComponents();
        }

        private void OnEnable()
        {
            ExecuteReferComponents();
        }

        public void ExecuteReferComponents()
        {
            if (_confiner != null)
            {
                ReferConfiner();
            }
            if (_vcam != null)
            {
                ReferFollow();
            }
        }

        private void ReferConfiner()
        {
            var found = CameraBoundNames
                    .Select(cb => GameObject.Find(cb))
                    .Where(cb => cb != null)
                    .FirstOrDefault();
            if (found == null)
            {
                Debug.LogError($"Cannot find camera bound in this scene.");
                return;
            }
            var boundCollider = found.GetComponent<Collider2D>();
            if (boundCollider == null)
            {
                Debug.LogError($"Not exists Collider2D component in object '{found.name}'.");
                return;
            }
            _confiner.m_BoundingShape2D = boundCollider;
        }

        private void ReferFollow()
        {
            var found = PlayerTags
                .Select(tag => GameObject.FindGameObjectWithTag(tag))
                .Where(obj => obj != null)
                .FirstOrDefault();
            if (found == null)
            {
                Debug.LogError($"Cannot find object of player tag in this scene.");
                return;
            }
            _vcam.Follow = found.transform;
        }
    }
}
