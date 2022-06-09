using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIreBall_Mortar : MonoBehaviour
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


    private float _lifeTimer;

    [SerializeField]
    private Rigidbody2D _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = transform.right * _speed;
    }

    // Update is called once per frame
    void Update()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer > _lifeTime)
        {
            for(int i = 1; i <= _childFireBallNum; i++)
            {
                Instantiate(_childFireBall, transform.position, Quaternion.Euler(0, 0, transform.rotation.z + _childFireBallSpread * i));
            }
            Destroy(gameObject);
        }
    }
}
