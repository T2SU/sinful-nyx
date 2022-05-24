using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sevens.Entities.Players;
using Sevens.Utils;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Sevens.UIs;
using Sevens.Speeches;

public class SetCameraPriority : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private PlayableDirector _playableDirector;

    [SerializeField]
    private SystemDialogue _systemDialogue;

    [SerializeField]
    private ScriptObject _scripts;

    enum DisplayStep { None, Waiting, Triggered }

    private DisplayStep _displayedMoveTutorial;

    private void Awake()
    {
        _displayedMoveTutorial = DisplayStep.None;
    }

    private void Start()
    {
        if (_player.Achievements.GetData(PlayerDataKeyType.TutorialStageEntered) != "1")
        {
            _virtualCamera.Priority = 11;
            _playableDirector.Play();
            _player.Achievements.SetData(PlayerDataKeyType.TutorialStageEntered, "1");
            _displayedMoveTutorial = DisplayStep.Waiting;
        }
        else
        {
            _virtualCamera.Priority = 9;
        }
    }

    public void StartIntroConversation()
    {
        StartCoroutine(DelayDialogue());
    }

    private IEnumerator DelayDialogue()
    {
        yield return DialogueManager.Instance.StartDialogue(_scripts.PlayScript());
        _displayedMoveTutorial = DisplayStep.Triggered;
        _systemDialogue.Display("����Ű<color=yellow>(���)</color>�� ���� <color=skyblue>�¿� �̵�</color> �� �� �ֽ��ϴ�.", 2);
    }
}
