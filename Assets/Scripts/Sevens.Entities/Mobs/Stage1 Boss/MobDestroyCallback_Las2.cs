using DG.Tweening;
using Sevens.Entities.Players;
using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobDestroyCallback_Las2 : EntityDestroyCallbackBase
    {
        public GameObject EssencePrefab;

        public override void Execute(Entity entity, Entity killedBy)
        {
            if (entity is Mob mob && killedBy is Player player)
            {
                StartCoroutine(EndDirectionTimeline(mob, player));
            }
        }

        private IEnumerator EndDirectionTimeline(Mob mob, Player player)
        {
            // BGM 끄기
            var bgm = GameObject.Find("BGM").GetComponent<AudioSource>();
            bgm.DOFade(0f, 1f);

            // 뒈짖 애니메이션 및 사운드 재생
            mob.PlayAnimation(new AnimationPlayOption("Die"), 
                immediatelyTransition: true);
            mob.PlayAudio("Die");

            var block = new MaterialPropertyBlock();

            var renderers = GetComponentsInChildren<MeshRenderer>();

            int fillPhase = Shader.PropertyToID("_FillPhase");
            int fillColor = Shader.PropertyToID("_FillColor");
            block.SetColor(fillColor, Color.black);
            foreach (var renderer in renderers)
                renderer.SetPropertyBlock(block);

            mob.PlayEffect("DieEffect", transform.position);
            DOTween.To(() => block.GetFloat(fillPhase), f => { 
                block.SetFloat(fillPhase, f);
                foreach (var renderer in renderers)
                    renderer.SetPropertyBlock(block);
            }, 0.75f, 3f);
            DOTween.To(() => mob.GetSkelAlpha(), f => mob.SetSkelAlpha(f), 0f, 5f).SetDelay(3f);

            yield return new WaitForSeconds(3f);

            var essense = Instantiate(EssencePrefab);
            var spr = essense.GetComponentInChildren<SpriteRenderer>();
            spr.DOColor(Color.white, 1.5f);

        }
    }
}
