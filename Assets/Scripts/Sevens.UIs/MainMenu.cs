using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : GeneralMenuController
{
    [SerializeField]
    private string sceneNewGame;

    public override void Awake()
    {
        sceneName = sceneNewGame;
        base.Awake();
    }
    public override void OnClickSettings()
    {
        base.OnClickSettings();
        int i = 2;

        settingsMenu.transform.GetChild(i + 7).gameObject.SetActive(true);
        for (i = 2; i < settingsGroup.Length; i++) {
            settingsGroup[i].gameObject.SetActive(true);
        }
    }

    public override void OnClickBack()
    {
        base.OnClickBack();
    }

    public override void OnClickExit()
    {
        base.OnClickExit();
    }
}