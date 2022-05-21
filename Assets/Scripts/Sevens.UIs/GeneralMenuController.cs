using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GeneralMenuController : MonoBehaviour, IMenuController
{
    Transform[] menuGroup;
    Image settingsBackground;
    Button[] menuButtonGroup;
    int menuButtonGroupIndex;
    public string sceneName;

    [HideInInspector]
    public Button[] settingsGroup;

    public SceneFader Fader;
    public Transform settingsMenu;

    private string currentSceneName {
        get => SceneManager.GetActiveScene().name;
    }

    public virtual void Awake() {
        InitialSetting();

        menuButtonGroup[menuButtonGroupIndex - 2].onClick.AddListener(OnClickSettings);
        menuButtonGroup[menuButtonGroupIndex - 1].onClick.AddListener(OnClickExit);
        settingsGroup[5].onClick.AddListener(OnClickBack);

        if(currentSceneName == "Main Menu") {
            menuButtonGroup[menuButtonGroupIndex - 4].onClick.AddListener(OnClickContinue);
            menuButtonGroup[menuButtonGroupIndex - 3].onClick.AddListener(OnClickNewGame);
        }
        else {
            menuButtonGroup[menuButtonGroupIndex - 5].onClick.AddListener(OnClickResume);
            menuButtonGroup[menuButtonGroupIndex - 4].onClick.AddListener(OnClickLastCheckPoint);
            menuButtonGroup[menuButtonGroupIndex - 3].onClick.AddListener(OnClickToTitle);
        }
    }

    private void InitialSetting() {
        GameObject settingsGroupUI = Instantiate(UIUtility.Instance.data.settingsGroupPrefab, transform.position, Quaternion.identity, transform);

        menuGroup = GameObject.Find("Menu Group").GetComponentsInChildren<Transform>();
        menuButtonGroup = menuGroup[2].GetComponentsInChildren<Button>();

        settingsBackground = settingsGroupUI.GetComponentInChildren<Image>();
        settingsGroup = settingsGroupUI.GetComponentsInChildren<Button>();

        for (int i = 0; i < settingsGroup.Length - 1; i++) {
            int index = i;
            settingsGroup[i].onClick.AddListener(() => OnClickSettingsMenu(index));
        }

        menuButtonGroupIndex = menuButtonGroup.Length;

        settingsBackground.gameObject.SetActive(false);
        for (int j = 0; j < settingsGroup.Length; j++) {
            settingsGroup[j].gameObject.SetActive(false);
        }
    }

    public void OnClickSettingsMenu(int index) {
        settingsMenu = transform.GetChild(transform.childCount - 1);

        for (int i = 0; i < 5; i++) {
            settingsMenu.transform.GetChild(i + 7).gameObject.SetActive(false);
        }

        settingsMenu.transform.GetChild(index + 7).gameObject.SetActive(true);
    }

    private void OnClickContinue() {
        StartCoroutine(Fader.FadeAndLoadScene(SceneFader.FadeDirection.In, _SceneManagement.Instance.GetScene()));
        _SceneManagement.Instance.shouldLoadFromJson = true;
    }

    private void OnClickNewGame() {
        StartCoroutine(Fader.FadeAndLoadScene(SceneFader.FadeDirection.In, sceneName));
        _SceneManagement.Instance.ResetPlayerData();
    }

    private void OnClickResume() {

    }

    private void OnClickLastCheckPoint() {

    }

    private void OnClickToTitle() {
        UIUtility.Instance.OnPopUpInitiated("메인 타이틀로 돌아가시겠습니까?",
        "예",
        "아니오",
        () => { SceneManager.LoadScene("Main Menu"); },
        UIUtility.Instance.OnPopUpClosed);
    }

    public virtual void OnClickSettings() {
        menuGroup[0].gameObject.SetActive(false);

        settingsBackground.gameObject.SetActive(true);

        settingsMenu = transform.GetChild(transform.childCount - 1);

        RectTransform settingsGroupUI = settingsBackground.GetComponentsInParent<RectTransform>()[1];
        StartCoroutine(UIUtility.Instance.UIObjectFade(settingsGroupUI, true));

    }

    public virtual void OnClickExit() {
        UIUtility.Instance.OnPopUpInitiated(
        "게임을 종료 하시겠습니까?",
        "예",
        "아니오",
#if UNITY_EDITOR
        () => { UnityEditor.EditorApplication.isPlaying = false; },
#else
        () => { Application.Quit(0); },
#endif
        UIUtility.Instance.OnPopUpClosed);
    }

    public virtual void OnClickBack() {
        RectTransform settingsGroupUI = settingsBackground.GetComponentsInParent<RectTransform>()[1];
        StartCoroutine(UIUtility.Instance.UIObjectFade(settingsGroupUI, false));

        menuGroup[0].gameObject.SetActive(true);

        Transform settingsMenu = transform.GetChild(transform.childCount - 1);

        for (int i = 0; i < settingsMenu.childCount; i++) {
            settingsMenu.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
