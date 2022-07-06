using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sevens.Entities;

public class FireBall_Mortar : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _lifeTime;

    [SerializeField]
    private float _childFireBallSpread;
    [SerializeField]
    private GameObject _childFireBall;
    [SerializeField]
    private int _childFireBallNum;

    [SerializeField]
    private float _lifeTimer;

    [SerializeField]
    private Rigidbody2D _rigidbody;

    private Entity source;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = transform.right * _speed;
        source = GameObject.Find("AngryGuardian").GetComponent<Entity>();
    }

    void Update()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer > _lifeTime)
        {
            for(int i = 0; i < _childFireBallNum; i++)
            {
                GameObject obj;
                if (transform.localEulerAngles.z >= 90)
                {
                    obj = Instantiate(_childFireBall, transform.position, Quaternion.Euler(0, 0, -180 + transform.localEulerAngles.z + (_childFireBallSpread * i - _childFireBallSpread)));
                }
                else
                {
                    obj = Instantiate(_childFireBall, transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + (_childFireBallSpread * i - _childFireBallSpread)));
                }
                Blow.SetAllBlowSourceAs(obj, source);
            }
            Destroy(gameObject);
        }
    }
}
