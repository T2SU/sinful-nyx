using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    

    /*
    public GameObject optionBaseUI;

    private RectTransform optionBaseRectTransform;
    private Image optionBaseImage;

    public bool isEnabled = false;
    private float optionBaseOriginalPosition;

    void Start() {
        optionBaseRectTransform = optionBaseUI.GetComponent<RectTransform>();
        optionBaseImage = optionBaseUI.GetComponent<Image>();
        optionBaseOriginalPosition = optionBaseRectTransform.anchoredPosition.x;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            isEnabled = isEnabled ? false : true;
            StartCoroutine(AnimateOptionBaseUI());
        }
    }

    IEnumerator AnimateOptionBaseUI() {
        float speed = 5f;
        float percent = 0;
        int dir = 1;

        while(percent >= 0) {
            if(percent > 1) {
                percent = 1;
            }

            dir = isEnabled ? 1 : -1;

            percent += Time.deltaTime * speed * dir;

            optionBaseRectTransform.anchoredPosition = Vector2.right * Mathf.Lerp(optionBaseOriginalPosition, optionBaseRectTransform.sizeDelta.x, percent);
            optionBaseImage.color = Color.Lerp(Color.clear, Color.black, percent);
            yield return null;
        }
    }
    */
}
