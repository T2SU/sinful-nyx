using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sevens.Entities.Players;
using Sevens.Utils;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Sevens.UIs;

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

    private void Update()
    {
        if (_displayedMoveTutorial == DisplayStep.Waiting)
        {
            if (_playableDirector.state != PlayState.Playing)
            {
                _displayedMoveTutorial = DisplayStep.Triggered;
                _systemDialogue.Display("방향키<color=yellow>(←→)</color>를 눌러 <color=skyblue>좌우 이동</color> 할 수 있습니다.", 2);
            }
        }
    }
}
