using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private bool _isClicked;

    private Collider2D _collider;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        _isClicked = Input.GetKey(KeyCode.DownArrow);
    }

    void FixedUpdate()
    {
        Vector2 vector1 = new Vector2(transform.position.x + _collider.bounds.size.x / 2, transform.position.y + _collider.bounds.size.y / 2 + 1.75f);
        Vector2 vector2 = new Vector2(transform.position.x - _collider.bounds.size.x / 2, transform.position.y + _collider.bounds.size.y / 2 + 2f);

        if(Physics2D.OverlapArea(vector1, vector2) && !_isClicked)
        {
            transform.gameObject.layer = 9;
        }
        else
        {
            transform.gameObject.layer = 0;
        }

        //if(_isClicked)
        //{
        //    _platformEffecter.surfaceArc = 0f;
        //}
        //else
        //{
        //    _platformEffecter.surfaceArc = 1f;
        //}
    }
}
