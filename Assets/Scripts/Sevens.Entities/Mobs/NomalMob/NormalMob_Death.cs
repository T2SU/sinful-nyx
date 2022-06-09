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
        private VisualEffect effect;

        public override void Execute(Entity entity, Entity killedBy)
        {
            var mob = entity.GetComponent<Mob>();
            var animator = mob.GetComponent<Animator>();

            animator.enabled = false;

        }
    }

}
