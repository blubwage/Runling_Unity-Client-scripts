﻿using System.Collections;
using Assets.Scripts.Launcher;
using Assets.Scripts.UI.SLA_Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SLA
{
    public class InitializeGameSLA : MonoBehaviour {

        // Attach scripts
        public LevelManagerSLA LevelManagerSla;
        public ScoreSLA ScoreSla;
        public ControlSLA ControlSla;
        public InGameMenuManagerSLA InGameMenuManagerSla;

        public GameObject PlayerPrefab;
        public GameObject LevelTextObject;
        public GameObject CountdownPrefab;
        public GameObject CurrentPrWindow;
        public GameObject Player;
        public Text CurrentPr;

        

        //set Spawnimmunity once game starts
        public void InitializeGame()
        {
            StartCoroutine(PrepareLevel());
        }

        IEnumerator PrepareLevel()
        {
            // Set current Level and movespeed
            GameControl.MoveSpeed = LevelManagerSla.GetMovementSpeed(GameControl.CurrentLevel);
            
            // Show level highscore and current level
            CurrentPr.text = HighScoreSLA.highScoreSLA[GameControl.CurrentLevel].ToString();
            var levelText = LevelTextObject.GetComponent<TextMeshProUGUI>();
            levelText.text = "Level " + GameControl.CurrentLevel;
            LevelTextObject.SetActive(true);
            CurrentPrWindow.SetActive(true);
            yield return new WaitForSeconds(2f);
            LevelTextObject.SetActive(false);
            CurrentPrWindow.SetActive(false);
            yield return new WaitForSeconds(1f);

            // Load drones and player

            Player = Instantiate(PlayerPrefab);
            GameControl.Dead = false;
            GameControl.IsInvulnerable = true;
            GameControl.IsImmobile = false;
            ControlSla.StopUpdate = false;
            LevelManagerSla.LoadDrones(GameControl.CurrentLevel);
            
            // Countdown
            for (var i = 0; i < 3; i++)
            {
                var countdown = Instantiate(CountdownPrefab, GameObject.Find("ScoreCanvas").transform);
                countdown.GetComponent<TextMeshProUGUI>().text = (3 - i).ToString();
                yield return new WaitForSeconds(1f);
                Destroy(countdown);
            }
            
            GameControl.IsInvulnerable = false;
            ScoreSla.StartScore();
            
        }
    }
}
