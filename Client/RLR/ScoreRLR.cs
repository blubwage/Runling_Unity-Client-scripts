﻿using Client.Scripts.Launcher;
using Game.Scripts.GameSettings;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Scripts.RLR
{
    public class ScoreRLR : MonoBehaviour
    {
        public GameObject CurrentScoreText;
        public GameObject CountDownText;
        public Text NewHighScoreText;
        public GameObject NewHighScoreObject;

        public float[] FinishTimeCurGame = new float[LevelManagerRLR.NumLevels];
        private ControlRLR _controlRLR;
        private int _timeLimit;
        private float _countdown;
        private float _timer;
        private int _difficultyMultiplier;
        private float _initializationTime;

        private void Awake()
        {
            _controlRLR = GetComponent<ControlRLR>();
            switch (GameControl.GameState.SetDifficulty)
            {
                case Difficulty.Normal:
                    _difficultyMultiplier = 1;
                    break;
                case Difficulty.Hard:
                    _difficultyMultiplier = 2;
                    break;
            }
        }

        public void StartTimer()
        {
            _timeLimit = 285 + 15 * _controlRLR.CurrentLevel;
            _initializationTime = Time.time;
            _countdown = _timeLimit;
        }

        private void Update()
        {
            if (_countdown > 0 && !GameControl.GameState.FinishedLevel)
            {
                _timer = Time.time - _initializationTime;
                if (GameControl.GameState.SetGameMode == GameMode.TimeMode)
                {
                    _countdown = _timeLimit - _timer;
                    CountDownText.GetComponent<TextMeshProUGUI>().text = "Countdown: " + (int)_countdown / 60 + ":" + (_countdown % 60).ToString("00.00");
                }
            }
        }

        public void AddScore()
        {
                _controlRLR.PlayerManager.TotalScore += _difficultyMultiplier * _controlRLR.CurrentLevel;
                CurrentScoreText.GetComponent<TextMeshProUGUI>().text = "Current Score: " + _controlRLR.PlayerManager.TotalScore;
                SetTimeModeHighScore();
        }

        public void AddRemainingCountdown()
        {
            _controlRLR.PlayerManager.TotalScore += (int) _countdown;
            _countdown = 0;
        }

        //Checks for a new highscore and saves it
        public void SetHighScore()
        {
            if (GameControl.GameState.FinishedLevel && GameControl.GameState.SetGameMode != GameMode.Practice)
            {
                FinishTimeCurGame[_controlRLR.CurrentLevel - 1] = _timer;

                switch (GameControl.GameState.SetDifficulty)
                {
                    case Difficulty.Normal:
                        if (_timer < GameControl.HighScores.HighScoreRLRNormal[_controlRLR.CurrentLevel] || GameControl.HighScores.HighScoreRLRNormal[_controlRLR.CurrentLevel] < 0.1f)
                        {
                            StartCoroutine(NewHighScore(_timer));
                            GameControl.HighScores.HighScoreRLRNormal[_controlRLR.CurrentLevel] = _timer;
                            PlayerPrefs.SetFloat("HighScoreRLRNormal" + _controlRLR.CurrentLevel,
                                GameControl.HighScores.HighScoreRLRNormal[_controlRLR.CurrentLevel]);
                        }
                        break;
                    case Difficulty.Hard:
                        if (_timer < GameControl.HighScores.HighScoreRLRHard[_controlRLR.CurrentLevel] || GameControl.HighScores.HighScoreRLRHard[_controlRLR.CurrentLevel] < 0.1f)
                        {
                            StartCoroutine(NewHighScore(_timer));
                            GameControl.HighScores.HighScoreRLRHard[_controlRLR.CurrentLevel] = _timer;
                            PlayerPrefs.SetFloat("HighScoreRLRHard" + _controlRLR.CurrentLevel,
                                GameControl.HighScores.HighScoreRLRHard[_controlRLR.CurrentLevel]);
                        }
                        break;
                }
            }
            if (GameControl.GameState.SetGameMode == GameMode.TimeMode)
            {
                SetTimeModeHighScore();
            }
            PlayerPrefs.Save();
        }

        public void SetTimeModeHighScore()
        {
            switch (GameControl.GameState.SetDifficulty)
            {
                case Difficulty.Normal:
                    if (_controlRLR.PlayerManager.TotalScore > GameControl.HighScores.HighScoreRLRNormal[0])
                    {
                        GameControl.HighScores.HighScoreRLRNormal[0] = _controlRLR.PlayerManager.TotalScore;
                    }
                    PlayerPrefs.SetFloat("HighScoreRLRNormalTimeMode", GameControl.HighScores.HighScoreRLRNormal[0]);
                    break;
                case Difficulty.Hard:
                    if (_controlRLR.PlayerManager.TotalScore >= GameControl.HighScores.HighScoreRLRHard[0])
                    {
                        GameControl.HighScores.HighScoreRLRHard[0] = _controlRLR.PlayerManager.TotalScore;
                    }
                    PlayerPrefs.SetFloat("HighScoreRLRHardTimeMode", GameControl.HighScores.HighScoreRLRHard[0]);
                    break;
            }
        }

        private IEnumerator NewHighScore(float timer)
        {
            var record = timer;
            NewHighScoreText.text = record > 60 ? "New Highscore: " + (int)record / 60 + ":" + (record % 60).ToString("00.00") : "New Highscore: " + record.ToString("f2");
            NewHighScoreObject.SetActive(true);
            yield return new WaitForSeconds(3);
            NewHighScoreObject.SetActive(false);
        }
    }
}
