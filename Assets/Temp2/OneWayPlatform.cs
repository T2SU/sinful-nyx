using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private bool _isClicked;
    //[SerializeField]
    //private float _platformDisableTimer = 0f;
    //[SerializeField]
    //private float _disableTime = 0.5f;

    private Collider2D _collider;
    private PlatformEffector2D _platformEffector;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _platformEffector = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        _isClicked = Input.GetKey(KeyCode.DownArrow);
    }

    void FixedUpdate()
    {
        if(_isClicked)
        {
            _platformEffector.surfaceArc = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(ContactPoint2D contactPoint in collision.contacts)
        {
            if(contactPoint.normal.y > -0.9)
            {
                gameObject.layer = 0;
            }
            else
            {
                gameObject.layer = 9;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        gameObject.layer = 9;
        _platformEffector.surfaceArc = 1f;
    }
}
