using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sevens.Entities.Players;
using Sevens.Utils;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class SetCameraPriority : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private PlayableDirector _playableDirector;

    private void Start()
    {
        if (_player.Achievements.GetData(PlayerDataKeyType.TutorialStageEntered) != "1")
        {
            _virtualCamera.Priority = 11;
            _playableDirector.Play();
            _player.Achievements.SetData(PlayerDataKeyType.TutorialStageEntered, "1");
        }
        else
        {
            _virtualCamera.Priority = 9;
        }
    }
}
