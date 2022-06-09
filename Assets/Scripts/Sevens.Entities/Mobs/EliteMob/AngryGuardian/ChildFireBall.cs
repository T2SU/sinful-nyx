using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildFireBall : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _lifeTime;
    [SerializeField]
    private Rigidbody2D _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = transform.right * _speed;
        Destroy(gameObject, _lifeTime);
    }
}
