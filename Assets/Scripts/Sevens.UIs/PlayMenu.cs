using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenu : GeneralMenuController
{
    public override void Awake()
    {
        base.Awake();
    }
    public override void OnClickSettings()
    {
        base.OnClickSettings();

        int i = 0;

        settingsMenu.transform.GetChild(i + 7).gameObject.SetActive(true);
        for (i = 0; i < settingsGroup.Length; i++) {
            settingsGroup[i].gameObject.SetActive(true);
        }
    }

    public override void OnClickExit() {
        UIUtility.Instance.OnPopUpInitiated("게임을 종료하시겠습니까? 저장되지 않은 데이터는 모두 사라집니다.",
        "예",
        "아니오",
#if UNITY_EDITOR
        () => { UnityEditor.EditorApplication.isPlaying = false; },
#else
        () => { Application.Quit(0); },
#endif
        UIUtility.Instance.OnPopUpClosed);
    }
}
