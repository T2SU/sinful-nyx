using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Sevens.Events
{
    public class InteractionGuide : MonoBehaviour
    {
        [SerializeField] private TextMesh _buttonKey;
        [SerializeField] private TextMesh _buttonDescription;

        public void SetText(string key, string desc)
        {
            _buttonDescription.text = desc;
            _buttonKey.text = key;
        }
    }
}
