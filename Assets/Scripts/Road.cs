using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private Player _player;
    private float _speed = 0;
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
        //roads are moving in the same diraction
        transform.Translate(Vector3.back * _speed * Time.deltaTime);
        //if a road is behind the player - destroy it
        if (transform.position.z <= -18f)
            Destroy(gameObject);
    }
    void CheckSpeedUpdate()
    {
        if (_player.Speed != _speed)
            _speed = _player.Speed;
    }
}
