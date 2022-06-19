using System.Collections;
using UnityEngine;
using Cinemachine;
using Sevens.Entities.Players;
using Sevens.Utils;
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

    private void Start()
    {
#if UNITY_EDITOR
        _player.Achievements.SetData(PlayerDataKeyType.TutorialStageEntered, "1");
        _virtualCamera.Priority = 9;
        return;
#endif
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

    public void StartIntroConversation()
    {
        _player.Achievements.SetData(PlayerDataKeyType.BlinkCompleted, "1");

        StartCoroutine(DelayDialogue());
        SaveManager.SaveGame();
    }

    private IEnumerator DelayDialogue()
    {
        yield return DialogueManager.Instance.StartDialogue(_scripts.PlayScript());
        _systemDialogue.Display("방향키<color=yellow>(←→)</color>를 눌러 <color=skyblue>좌우 이동</color> 할 수 있습니다.", 2);
    }

}
