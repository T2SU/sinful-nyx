using UnityEngine;
using UnityEngine.UI;

namespace Sevens.UIs
{
    public class RadialGrowingGauge : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void UpdateRatio(float ratio)
        {
            _image.fillAmount = ratio;
        }
    }
}