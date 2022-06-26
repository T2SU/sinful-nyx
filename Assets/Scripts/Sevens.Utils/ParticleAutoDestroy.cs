using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(_particleSystem)
        {
            if(!_particleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
