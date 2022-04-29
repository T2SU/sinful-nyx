using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnToolTip : MonoBehaviour
{
    Slider[] soundSlider;

    public void OnPointerEnter() {
        Image image = GetComponent<Image>();
        Color transparent = new Color(1, 1, 1, 1);
        image.color = transparent;
    }

    public void OnPointerExit() {
        Image image = GetComponent<Image>();
        Color transparent = new Color(1, 1, 1, 0);
        image.color = transparent;
    }
}
