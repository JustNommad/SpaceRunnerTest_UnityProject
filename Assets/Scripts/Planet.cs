using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Asteroid
{
    void Update()
    {
        CheckSpeedUpdate();
        transform.Translate(Vector3.back * _speed * Time.deltaTime);
    }
}
