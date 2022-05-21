using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Entities;
using Sevens.Speeches;
using Sevens.Entities.Players;
using Sevens.Utils;
using Sevens.Interfaces;

public class SavePointEntity : InteractableEntity
{
    private bool isFirstContacted;

    [SerializeField]
    private GameObject particleBlink;
    [SerializeField]
    private GameObject particleGlow;
    private DialogueBase dialogueBase;
    [SerializeField]
    private LayerMask playerLayer;

    [SerializeField] Player player;

    protected override void Start()
    {
        var playerObj = GameObject.Find("Player");

        if (playerObj == null)
        {
            return;
        }

        player = playerObj.GetComponent<Player>();
    }

    protected override void Update() {
        base.Update();
        if(Physics2D.OverlapCircle(transform.position, 4, playerLayer) && isClicked) {
            Interact();
        }
    }
    protected override void Interact() {
        if (player.Achievements.GetData(PlayerDataKeyType.FirstContactCompleted) != "1") {
            StartCoroutine(FirstContactDialogue());
        }
        else {
            UIManager.Instance.Popup("저장 하시겠습니까?", "예", "아니오", () => {
                SaveManager.SaveGame();
                DialogueManager.Instance.DisplayHudMessage("데이터가 저장되었습니다.");
            });
        }
    }

    private IEnumerator FirstContactDialogue()
    {
        yield return DialogueManager.Instance.StartDialogue(FirstContactDialogue1());
        player.SetDirectionMode(true);
        yield return DelayedParticle();
        yield return DialogueManager.Instance.StartDialogue(FirstContactDialogue2());
        player.Achievements.SetData(PlayerDataKeyType.FirstContactCompleted, "1");
    }

    private IEnumerator FirstContactDialogue1() {
        yield return new Dialogue("닉스", null, "눈 먼 천사 석상... 이게 왜 이런 곳에...?");
        yield return new Dialogue("닉스", null, "분명 주인님이 #red'잃어버렸다'#white고 하셨는데...");
        yield return new Dialogue("석상의 하단부", null, "#bold'소원을 들어드립니다. 도전하시겠습니까?'");
        yield return new Dialogue("닉스", null, "그래, 중요한 건 그게 아니야. 나는 주인님을 살려야 해.");
        yield return new Dialogue("닉스", null, "당신이 정말 전설 속 소원을 이루어주는 천사라면, 뭐든 도전 할게요.");
    }

    private IEnumerator FirstContactDialogue2() {
        yield return new Dialogue("천사", null, "도전자여, 생기없는 몸에 혼이 깃든 도전자여. 소중한 이를 살리고자 한다면 죄악의 벽을 넘어라.");
        yield return new Dialogue("천사", null, "분노, 폭식, 질투, 탐욕, 색욕, 나태, 오만. 그 벽 너머에 해답이 있을 것이니.");
        yield return new Dialogue("천사", null, "그대의 #yellow'도전'#white에 행운이 함께하길.");
    }

    private IEnumerator DelayedParticle() {
        particleBlink.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        particleGlow.SetActive(true);
        yield return new WaitForSeconds(1.0f);
    }
}
