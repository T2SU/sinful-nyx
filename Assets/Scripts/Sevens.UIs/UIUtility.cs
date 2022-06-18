using Sevens.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIUtility : MonoBehaviour
{
    private static UIUtility instance;

    public static UIUtility Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject("UIUtility");
                instance = obj.AddComponent<UIUtility>();
            }
            return instance;
        }
    }

    private Transform popUp;

    public UIData data;

    private void Awake()
    {
        data = Resources.Load<UIData>(Prefabs.UIData);
    }

    public void OnPopUpInitiated(string popUpText, string selectiveOptionOne, string selectiveOptionTwo, Action ButtonAction1, Action ButtonAction2)
    {
        var playerUI = GameObject.Find("Canvas").transform;
        if (popUp != null)
        {
            Destroy(popUp.gameObject);
        }
        popUp = Instantiate(UIUtility.Instance.data.popUpObject, playerUI).transform;

        //StartCoroutine(UIObjectFade(popUp, true));

        Text[] popUpTexts = popUp.GetComponentsInChildren<Text>();
        popUpTexts[0].text = popUpText;

        Button[] popUpButton = popUp.GetComponentsInChildren<Button>();

        popUpTexts[1].text = selectiveOptionOne;
        popUpTexts[2].text = selectiveOptionTwo;
        
        popUpButton[0].onClick.AddListener(() => ButtonAction1());
        popUpButton[1].onClick.AddListener(() => ButtonAction2());
    }

    public void OnPopUpClosed()
    {
        //StartCoroutine(UIObjectFade(popUp, false));

        Text[] popUpTexts = popUp.GetComponentsInChildren<Text>();
        popUpTexts[0].text = null;

        Button[] popUpButton = popUp.GetComponentsInChildren<Button>();

        popUpTexts[1].text = null;
        popUpTexts[2].text = null;
        
        popUpButton[0].onClick.RemoveAllListeners();
        popUpButton[1].onClick.RemoveAllListeners();

        Destroy(popUp.gameObject);
    }

    public IEnumerator UIObjectFade(Transform targetObject, bool fadeIndex)
    {
        targetObject.gameObject.AddComponent<CanvasGroup>();

        CanvasGroup targetFade = targetObject.GetComponent<CanvasGroup>();

        if(fadeIndex == false)
        {
            targetFade.alpha = 1;

            while(targetFade.alpha >= 0)
            {
                targetFade.alpha -= 0.1f;
                yield return new WaitForSeconds(0.025f);
            }
        }
        else
        {
            targetFade.alpha = 0;

            while(targetFade.alpha <= 1)
            {
                targetFade.alpha += 0.1f;
                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}
