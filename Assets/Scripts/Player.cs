using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainCamera;
    [SerializeField]
    private GameObject _flashLight;
    [SerializeField]
    private GameObject _engineEffect;
    [SerializeField]
    private AudioClip _explosion;
    [SerializeField]
    private AudioClip _boostSound;
    [SerializeField] 
    private AudioClip _buttonSound;
    [SerializeField]
    private float _defaultHorizontalSpeed = 4.0f;
    [SerializeField]
    private float _speedBoost = 2.0f;
    [SerializeField]
    private float _timeForBoost = 2.0f;
    [SerializeField]
    private float _defaultSpeed = 5.0f;
    [SerializeField]
    private float _asteroidDistanceSpawnFrom = 6.0f;
    [SerializeField]
    private float _asteroidDistanceSpawnTo = 10.0f;
    [SerializeField]
    private int _step = 50;
    [SerializeField]
    private iTween.EaseType _easeType;
    [SerializeField]
    private iTween.LoopType _loopType;

    //Player stats scores/asteroids/time
    private int _score = 0;
    private int _asteroid = 0;
    private int _highestScore = 0;
    private int _lastStep = 0;          //this variable is needed to determine the boundary of the next level of difficulty 
    private float _startTime = 0f;
    private float _roundTime = 0f;
    private float _bestTime = 0f;
    private double _tempSec = 0;        

    private float _horizontalSpeed = 0;
    private bool _isBoostActive = false;
    private Vector3 _defaultPosition;
    private float _soundVolume = 1.0f;
    private float _soundMusicVolume = 1.0f;

    private SpawnManager _spawnManager;
    private RoadManager _roadManager;
    private SmoothFollow _smoothFollow;
    private UIManager _uiManager;
    private SaveLoadGame _saveLoadGame;
    private AudioSource _audioSource;
    private AudioSource _mainSource;

    public float SoundVolume
    {
        get => _soundVolume;
        set
        {
            _soundVolume = value;
            _audioSource.volume = _soundVolume;
        }
    }

    public float SoundMusicVolume
    {
        get => _soundMusicVolume;
        set
        {
            _soundMusicVolume = value;
            _mainSource.volume = _soundMusicVolume;
        }
    }
    public bool IsOption { get; set; }

    public bool IsAlive { get; private set; }
    public float Speed { get; private set; }
    public float AsteroidDistanceSpawnFrom { get; private set; }
    public float AsteroidDistanceSpawnTo { get; private set; }
    public int numberOfBlocks = 20;
    void Start()
    {
        InitializeComponents();
        //loading last save file
        LoadGame();
    }
    void Update()
    {
        if (IsAlive)
        {
            CalculateMovement();
            CalculateRotate();
            Boost();
            TimeUpdate();
            CalculateScore();
            DifficultyUpgrade();
        }
        else if(!IsOption)
            StartGame();
    }
    void DifficultyUpgrade()
    {
        //if there is enough scores for next difficulty then we increase it 
        if(_lastStep <= _score)
        {
            Speed += 5.0f;
            _defaultSpeed += 5.0f;
            _lastStep += _step;
            _step += 50;

            if (_defaultHorizontalSpeed < 10.0f)
                _defaultHorizontalSpeed += 1.0f;
            //changing asteroid distance
            if(_asteroidDistanceSpawnFrom >= 16.0f && _asteroidDistanceSpawnTo >= 22.0f)
            {
                AsteroidDistanceSpawnFrom -= 2.0f;
                AsteroidDistanceSpawnTo -= 2.0f;
            }
        }
    }
    void CalculateMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        Vector3 directional = new Vector3(horizontalMovement, 0, 0);
        Vector3 diractionalFlashlight = new Vector3(0, horizontalMovement, 0);
        // if the player will press A/D button or Arrow Left/Right Button then changing position
        transform.Translate(directional * _horizontalSpeed * Time.deltaTime);
        _flashLight.transform.Translate(diractionalFlashlight * _horizontalSpeed * Time.deltaTime);
        //enter player position but do not let the player out of the border
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.7f, 1.7f), _defaultPosition.y, _defaultPosition.z);
        _flashLight.transform.position = new Vector3(Mathf.Clamp(_flashLight.transform.position.x, -1.7f, 1.7f), _flashLight.transform.position.y, _flashLight.transform.position.z);
    }
    void Boost()
    {
        //if player pressed Space button then activate boost
        if (Input.GetKeyDown(KeyCode.Space) && !_isBoostActive)
        {
            _isBoostActive = true;
            _audioSource.PlayOneShot(_boostSound, _soundVolume);
            Speed = _defaultSpeed * _speedBoost;
            _horizontalSpeed = _defaultHorizontalSpeed + 2.0f;
            ChangeCameraPosition(true);             //changing camera position when a boost is active
            StartCoroutine(SpeedBoost());           //start the timer counting down the duration of the bonus
        }
        else if (!_isBoostActive)
        {
            Speed = _defaultSpeed;
            _horizontalSpeed = _defaultHorizontalSpeed;
            ChangeCameraPosition(false);
        }
    }
    void CalculateRotate()
    {
        //rotate the ship from his position to the end point. Left, right or idle
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            iTween.RotateTo(gameObject, iTween.Hash("z", -23, "time", 0.5f, "easetype", _easeType, "looptype", _loopType));
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            iTween.RotateTo(gameObject, iTween.Hash("z", 23, "time", 0.5f, "easetype", _easeType, "looptype", _loopType));
        else
            iTween.RotateTo(gameObject, iTween.Hash("z", 0, "time", 0.5f, "easetype", _easeType, "looptype", _loopType));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Asteroid")
        {
            AudioSource.PlayClipAtPoint(_explosion, transform.position, 1.0f);
            //if the player hits an asteroid then GameOver
            GameOver();
        }
    }
    void InitializeComponents()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("Player: _spawnManager is NULL");

        _roadManager = GameObject.Find("RoadManager").GetComponent<RoadManager>();
        if (_roadManager == null)
            Debug.Log("Player: _roadManager is NULL");

        _smoothFollow = _mainCamera.GetComponent<SmoothFollow>();
        if (_smoothFollow == null)
            Debug.Log("Player: _smoothFollow is NULL");

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.Log("Player: _uiManager is NULL");

        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.Log("Player: _audioSource is NULL");

        _mainSource = _mainCamera.GetComponent<AudioSource>();

        _soundVolume = _audioSource.volume;
        _soundMusicVolume = _mainSource.volume;

        //creating a saveFile 
        _saveLoadGame = new SaveLoadGame();
        _defaultPosition = transform.position;
    }
    IEnumerator SpeedBoost()
    {
        yield return new WaitForSeconds(_timeForBoost);
        _isBoostActive = false;
    }
    void ChangeCameraPosition(bool look)
    {
        if(look)
        {
            _smoothFollow.Distance = 1.5f;
            _smoothFollow.height = 1.2f;
        }
        else
        {
            _smoothFollow.Distance = _smoothFollow.distance;
            _smoothFollow.height = 2.0f;
        }
    }
    void TimeUpdate()
    {
        _roundTime = Time.time - _startTime;
        if (_roundTime >= _bestTime)
            _bestTime = _roundTime;

        _uiManager.time = _roundTime;
        _uiManager.bestTime = _bestTime;
    }
    void CalculateScore()
    {
        //Update score every second
        if (_tempSec < Math.Floor(_roundTime))
        {
            if (_isBoostActive)
                AddScore(2);
            else
                AddScore(1);
            _tempSec = Math.Floor(_roundTime);
        }
        if (_score >= _highestScore)
            _highestScore = _score;

        //Update UI 
        _uiManager.asteroid = _asteroid;
        _uiManager.score = _score;
        _uiManager.hightScore = _highestScore;
    }
    public void AddScore(int score, int asteroid = 0)
    {
        _score += score;
        _asteroid += asteroid;
    }
    private void SaveGame()
    {
        //adding a data in SaveFile object
        _saveLoadGame.HighestScore = _highestScore;
        _saveLoadGame.BestTime = _bestTime;

        _saveLoadGame.SaveGame();
    }
    private void LoadGame()
    {
        //preparing to the loading savefale from computer
        bool result = _saveLoadGame.LoadGame();
        if(result)
        {
            _highestScore = _saveLoadGame.HighestScore;
            _bestTime = _saveLoadGame.BestTime;
        }
    }
    private void GameOver()
    {
        IsAlive = false;
        Speed = 0;
        _spawnManager.CanSpawn = false;
        SaveGame();
        _uiManager.GameOver();
        Destroy(gameObject);
    }
    private void StartGame()
    {
        if (Input.GetKeyDown(KeyCode.O) && !IsOption)
        {
            _uiManager.OptionsUI();
            IsOption = true;
            return;
        }
        //change UI
        _uiManager.StartGame();
        if(Input.anyKey)
        {
            _audioSource.PlayOneShot(_buttonSound, _soundVolume);
            
            IsAlive = true;
            _spawnManager.CanSpawn = true;
            _audioSource.Play();
            _roadManager.RoadEnebled(true);
            _engineEffect.SetActive(true);

            Speed = _defaultSpeed;
            AsteroidDistanceSpawnFrom = _asteroidDistanceSpawnFrom;
            AsteroidDistanceSpawnTo = _asteroidDistanceSpawnTo;
            _startTime = Time.time;
            _lastStep = _step;

            //making player model and flashlight visible
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            _flashLight.GetComponent<MeshRenderer>().enabled = true;

            //change UI
            _uiManager.GamePlay();
        }
    }
}
