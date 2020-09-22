using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _asteroidPrefab;
    [SerializeField]
    private GameObject _asteroidContainer;

    private Player _player;
    private GameObject _lastAsteroid = null;
    public bool CanSpawn { get; set; }
    private float distanceRange = 0;
    private float _asteroidDistanceSpawnFrom = 0;
    private float _asteroidDistanceSpawnTo = 0;


    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.Log("SpawnManager: _player is NULL");
        CanSpawn = false;
    }

    void Update()
    {
        UpdateAsteroidRange();
        if(CanSpawn)
            SpawnAsteroid();
    }
    void SpawnAsteroid()
    {
        //point of spawn
        float positionZ = (_player.numberOfBlocks / 2) * 6f;
        if (_lastAsteroid != null)
        {
            //distance bitween last asteroid and point of spawn
            float distance = positionZ - _lastAsteroid.transform.position.z;
            if(distance >= distanceRange)
            {
                AddNewAsteroid(positionZ);
            }
        }
        else
            AddNewAsteroid(positionZ);
    }
    void AddNewAsteroid(float positionZ)
    {
        float randomX = Random.Range(-1.7f, 1.7f);
        Vector3 placeToSpawn = new Vector3(randomX, _asteroidPrefab.transform.position.y, positionZ);
        GameObject newEnemy = Instantiate(_asteroidPrefab, placeToSpawn, Quaternion.identity);
        newEnemy.transform.parent = _asteroidContainer.transform;
        _lastAsteroid = newEnemy;
        distanceRange = Random.Range(_asteroidDistanceSpawnFrom, _asteroidDistanceSpawnTo);
    }
    //updating data on the distance at which we can spawn asteroids from each other
    void UpdateAsteroidRange()
    {
        if (_asteroidDistanceSpawnFrom != _player.AsteroidDistanceSpawnFrom || _asteroidDistanceSpawnTo != _player.AsteroidDistanceSpawnTo)
        _asteroidDistanceSpawnFrom = _player.AsteroidDistanceSpawnFrom;
        _asteroidDistanceSpawnTo = _player.AsteroidDistanceSpawnTo;
    }
}
