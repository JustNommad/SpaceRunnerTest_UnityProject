using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeZone : MonoBehaviour
{
    private Player _player;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.Log("PlayerFreeZone: _player is NULL");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Asteroid")
        {
            _player.AddScore(5, 1);
        }
    }
}
