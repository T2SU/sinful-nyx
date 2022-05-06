using DG.Tweening;
using Sevens.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Sevens.Interfaces
{
    [RequireComponent(typeof(UIScroller))]
    public class IngameMenuController : MonoBehaviour
    {
        private UIScroller _uiScroller;

        public void OnClickResume()
        {
            _uiScroller.PlayBackward();
        }

        public void OnClickLastCheckpoint()
        {

        }

        public void OnClickToTitle()
        {

        }

        public void OnClickExit()
        {

        }

        private void Awake()
        {
            _uiScroller = GetComponent<UIScroller>();
        }
    }
}
