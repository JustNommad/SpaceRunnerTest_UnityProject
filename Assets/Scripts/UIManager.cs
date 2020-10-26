using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _timeText;
    [SerializeField]
    private Text _bestTimeText;
    [SerializeField]
    private Text _bestScoreText;
    [SerializeField]
    private Text _asteroidText;
    [SerializeField]
    private Text _gameOver;
    [SerializeField]
    private Text _record;
    [SerializeField]
    private Text _exit;
    [SerializeField]
    private Text _restart;
    [SerializeField]
    private Text _endScore;
    [SerializeField]
    private Text _endAsteroid;
    [SerializeField]
    private Text _endTime;
    [SerializeField]
    private Text _name;
    [SerializeField]
    private Text _start;
    [SerializeField]
    private GameObject _gameOverUI;
    [SerializeField]
    private GameObject _startUI;
    [SerializeField]
    private GameObject _gamePlayUI;
    [SerializeField] 
    private GameObject _optionsUI;
    [SerializeField] 
    private Slider _musicSlider;
    [SerializeField] 
    private Slider _effectSlider;
    [SerializeField] 
    private Player _player;


    public int score = 0;
    public int hightScore = 0;
    public float time = 0;
    public float bestTime = 0;
    public int asteroid = 0;
    void Start()
    {
        //taking a data from save file
        _scoreText.text = "Score: " + score;
        _bestScoreText.text = "Highest score: " + hightScore;
        _timeText.text = "Time: " + ConvertToTimer(time);
        _bestTimeText.text = "Best time: " + ConvertToTimer(bestTime);
    }

    void Update()
    {
        UpdateUI();
        if (_gameOverUI.activeSelf)
            RestartGame();
        if(_optionsUI.activeSelf)
            Options();
            
    }
    //upgrade UI when the game is running
    void UpdateUI()
    {
        _scoreText.text = "Score: " + score;
        _asteroidText.text = "Asteroids: " + asteroid;
        if (hightScore <= score)
            _bestScoreText.text = "Highest score: " + score;
        else
            _bestScoreText.text = "Highest score: " + hightScore;

        _timeText.text = "Time: " + ConvertToTimer(time);
        if (time >= bestTime)
            _bestTimeText.text = "Best time: " + ConvertToTimer(time);
        else
            _bestTimeText.text = "Best time: " + ConvertToTimer(bestTime);
    }
    string ConvertToTimer(float seconds)
    {
        //Convert a seconds to usual form
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        var result = t.ToString(@"mm\:ss");
        return result;
    }
    public void GameOver()
    {
        _gamePlayUI.SetActive(false);
        _startUI.SetActive(false);
        _optionsUI.SetActive(false);
        //update a UI manager with new data
        if (bestTime <= time || hightScore <= score)
            _record.enabled = true;
        else
            _record.enabled = false;

        _endAsteroid.text = "Asteroids\n\n" + asteroid;
        _endScore.text = "Score\n\n" + score;
        _endTime.text = "Time\n\n" + ConvertToTimer(time);

        _gameOverUI.SetActive(true);

    }
    public void GamePlay()
    {
        _startUI.SetActive(false);
        _gameOverUI.SetActive(false);
        _optionsUI.SetActive(false);
        _gamePlayUI.SetActive(true);
    }
    public void StartGame()
    {
        _gameOverUI.SetActive(false);
        _gamePlayUI.SetActive(false);
        _optionsUI.SetActive(false);
        _startUI.SetActive(true);
    }
    private void RestartGame()
    {
        //when player failed, if he press R button than game will start again
        if (Input.GetKeyDown(KeyCode.R))
        {
            var index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void OptionsUI()
    {
        _gameOverUI.SetActive(false);
        _gamePlayUI.SetActive(false);
        _startUI.SetActive(false);
        _effectSlider.value = _player.SoundVolume;
        _musicSlider.value = _player.SoundMusicVolume;
        _optionsUI.SetActive(true);
    }

    private void Options()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _player.IsOption = false;
    }
}
