using DG.Tweening;
using Sevens.Speeches;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Interfaces
{
    [RequireComponent(typeof(UIScroller))]
    public class IngameMenuController : MonoBehaviour
    {
        private UIScroller _uiScroller;

        public void OnClickResume()
        {
            _uiScroller.PlayBackward();
        }

        public void OnClickLastCheckpoint()
        {
            UIManager.Instance.Popup("마지막 체크포인트로 이동하시겠습니까?\n\n<color=red>저장되지 않은 데이터는 모두 사라집니다.</color>", "예", "아니오", LoadLastCheckpoint);
        }

        public void OnClickToTitle()
        {
            UIManager.Instance.Popup("타이틀 화면으로 돌아가시겠습니까?\n\n<color=red>저장되지 않은 데이터는 모두 사라집니다.</color>", "예", "아니오", ReturnToTitle);
        }

        public void OnClickExit()
        {
            UIManager.Instance.Popup("게임을 종료하시겠습니까?\n\n<color=red>저장되지 않은 데이터는 모두 사라집니다.</color>", "예", "아니오", ExitGame);
        }

        private void Awake()
        {
            _uiScroller = GetComponent<UIScroller>();
        }

        private void LoadLastCheckpoint()
        {
            Debug.Log("마지막 체크포인트 이동 - 예 선택됨");
            DialogueManager.Instance.StartDialogue(TestDialogue());
        }

        private IEnumerator TestDialogue()
        {
            yield return new Dialogue("테스트", null, "안녕하세요.0");
            yield return new Dialogue("테스트", null, "안녕하세요.1");
            yield return new Dialogue("테스트", null, "안녕하세요.2");
            yield return new Dialogue("테스트", null, "안녕하세요.3");
        }

        private void ReturnToTitle()
        {
            Debug.Log("타이틀 화면으로 - 예 선택됨");
        }

        private void ExitGame()
        {
            Debug.Log("게임 종료 - 예 선택됨");
        }
    }
}
