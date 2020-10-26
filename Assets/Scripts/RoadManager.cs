using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _roadObject;
    [SerializeField]
    private GameObject _roadContainer;

    private Player _player;

    private GameObject _lastRoad;               //last road which was created
    private int _numberOfBlocks = 0;            //maximum roads at the same time
    private float _startPointToSpawn = -12f;
    void Start()
    {
        InitializeRoad();
    }

    void FixedUpdate()
    {
        CheckBlocks();
    }
    public void RoadEnebled(bool active)
    {
        _roadContainer.SetActive(active);
    }
    void InitializeRoad()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player != null)
        {
            _numberOfBlocks = _player.numberOfBlocks;
        }
        else
            Debug.Log("RoadManager: _player is NULL");

        // build the road before starting 
        if (_roadObject != null)
        {
            for (int i = 0; i < _numberOfBlocks; i++)
            {
                AddNewBlock(_startPointToSpawn + (i * 6f));
            }
        }
        else
            Debug.Log("_roadBlock is NULL");
    }
    void CheckBlocks()
    {
        //if roads are less than the maximum - add new road
        if (_roadContainer.transform.childCount < _numberOfBlocks)
        {
            float lastBlockZPosition = _lastRoad.transform.position.z;
            AddNewBlock(lastBlockZPosition + 6f);
        }
    }
    void AddNewBlock(float positionZ)
    {
        //create a coordinates where will spawn a road, add it in perents and remember the last gameobject
        Vector3 placeToSpawn = new Vector3(_roadObject.transform.position.x, _roadObject.transform.position.y, positionZ);
        GameObject newBlock = Instantiate(_roadObject, placeToSpawn, Quaternion.identity);
        newBlock.transform.parent = _roadContainer.transform;
        _lastRoad = newBlock;
    }
}
