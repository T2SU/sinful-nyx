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
    private PlayableDirector _playable;

    private int _priority;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (_player.Achievements.GetData(PlayerDataKeyType.TutorialStageEntered) != "1")
        {
            _playable.Play();
            _player.Achievements.SetData(PlayerDataKeyType.TutorialStageEntered, "1");
        }
        else
        {
            _priority = 9;
        }

        _virtualCamera.Priority = _priority;
    }
}
