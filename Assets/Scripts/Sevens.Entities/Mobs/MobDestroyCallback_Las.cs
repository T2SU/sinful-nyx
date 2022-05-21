using DG.Tweening;
using Sevens.Effects;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sevens.Entities.Mobs
{
    public class MobDestroyCallback_Las : EntityDestroyCallbackBase
    {
        public GameObject CurtainBase;
        public Color CurtainColor;
        public CameraShakeOption CameraShake;
        public GameObject ShakeEffect;
        public string NextSceneName;
        public string NextSceneBossObjectName;

        private CoroutineMan _coroutines;

        public override void Execute(Entity entity, Entity killedBy)
        {
            if (killedBy is Player player)
            {
                //TODO set player to invincible
                //player.SetInvincible(true);
                _coroutines.Register("Revive", ReviveDirectionTimeline(entity, player));
            }
        }

        private IEnumerator ReviveDirectionTimeline(Entity entity, Player player)
        {
            if (!(entity is Mob mob))
                yield break;

            var time = mob.PlayAnimation(new AnimationPlayOption("Die"), immediatelyTransition: true);
            mob.PlayAudio("Die");
            var bgm = GameObject.Find("BGM").GetComponent<AudioSource>();
            bgm.DOFade(0f, 2f);

            //TODO Lock player input
            player.SetDirectionMode(true);

            yield return new WaitForSeconds(time);
            var cam = Camera.main;
            var curtain = Instantiate(CurtainBase, cam.transform);
            curtain.name = "SceneCurtain";
            var spr = curtain.GetComponent<SpriteRenderer>();
            spr.color = Color.clear;
            mob.Camera.Shake(CameraShake);
            if (ShakeEffect != null)
                ShakeEffect.SetActive(true);
            yield return new WaitForSeconds(1.0f);

            spr.DOColor(CurtainColor, 3.0f);
            yield return new WaitForSeconds(3.5f);
            SceneManagement.Instance.LoadScene(NextSceneName);
        }

        private void Awake()
        {
            _coroutines = new CoroutineMan(this);
        }

        private void OnDestroy()
        {
            _coroutines.KillAll();
        }
    }
}
