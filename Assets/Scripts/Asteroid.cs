using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    protected Player _player;
    protected float _speed = 0;
    private float _rotateSpeed = 100.0f;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.Log($"Road: _player is NULL");

        _speed = _player.Speed;
    }
    void Update()
    {
        CheckSpeedUpdate();
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.back * _speed * Time.deltaTime);
        transform.Rotate(Vector3.back * _rotateSpeed * Time.deltaTime);
        //if a road is behind the player - destroy it
        if (transform.position.z <= -18f)
            Destroy(gameObject);
    }
    protected void CheckSpeedUpdate()
    {
        if (_player.Speed != _speed)
            _speed = _player.Speed;
    }
}
