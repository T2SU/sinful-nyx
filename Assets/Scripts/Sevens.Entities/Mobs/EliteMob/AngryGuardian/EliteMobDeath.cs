using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Entities.Mobs;
using Sevens.Entities;
using UnityEngine.VFX;

public class EliteMobDeath : EntityDestroyCallbackBase
{
    [SerializeField]
    private VisualEffect _effect;
    [SerializeField]
    private float _fadeTimeMultiplier;
    [SerializeField]
    private float _fadeTimer = 1f;

    public override void Execute(Entity entity, Entity killedBy)
    {
        StartCoroutine(Fade());

        var animator = GetComponent<Animator>();
        animator.enabled = false;

        _effect.Play();
    }

    private IEnumerator Fade()
    {
        while(_fadeTimer > 0)
        {
            var renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat("_Fade", _fadeTimer);
            }

            _fadeTimer -= Time.deltaTime * _fadeTimeMultiplier;

            yield return null;
        }

        _effect.Stop();
        Destroy(gameObject);
    }
}
