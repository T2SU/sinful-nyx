using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Sevens.Entities.Mobs
{
    public class NormalMob_Death : EntityDestroyCallbackBase
    {
        [SerializeField]
        private Sprite[] sprites;
        [SerializeField]
        private Transform _pivotTransform;
        [SerializeField]
        private VisualEffect _effect;
        [SerializeField, Header("Fade Time Multiplier, 0 ~ 1")]
        private float fadeTimeMultiplier;
        [SerializeField]
        private float fadeTimer = 1f;

        private void Start()
        {
            _effect.Stop();
        }

        public override void Execute(Entity entity, Entity killedBy)
        {
            StartCoroutine(Fade());

            var mob = entity.GetComponent<Mob>();
            var animator = mob.GetComponent<Animator>();

            animator.enabled = false;

            _effect.SetVector3("MobSize", transform.localScale);

            _effect.Play();
        }

        private IEnumerator Fade()
        {
            while (fadeTimer > 0)
            {
                var renderers = GetComponentsInChildren<SpriteRenderer>();

                foreach (var renderer in renderers)
                {
                    var block = new MaterialPropertyBlock();

                    renderer.GetPropertyBlock(block);

                    //block.SetColor("_TexColor", Color.black);
                    block.SetFloat("_Fade", fadeTimer);
                    

                    renderer.SetPropertyBlock(block);
                }

                fadeTimer -= Time.deltaTime * fadeTimeMultiplier;

                yield return null;
            }

            _effect.Stop();
        }
    }

}
