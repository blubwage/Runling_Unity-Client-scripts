﻿using DarkRift;
using Launcher;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network.Login
{
    public class Login : MonoBehaviour
    {
        public InputField UsernameInput;
        public InputField PasswordInput;

        public GameObject ControlPanel;
        public Text ConnectionText;
        public Text FeedbackText1;
        public Text FeedbackText2;
        public LoadingAnimation LoadingAnimation;
        public Button LoginButton;
        public Button AddUserButton;
        public Button OfflineButton;
        public GameObject CancelButton;
        public Toggle RememberPassword;

        private bool _rememberPassword;

        private void Awake()
        {
            _rememberPassword = PlayerPrefs.GetInt("RememberPassword") != 0;
            RememberPassword.isOn = _rememberPassword;
            CancelButton.SetActive(false);

            if (PlayerPrefs.HasKey("username"))
                UsernameInput.text = PlayerPrefs.GetString("username");
            if (PlayerPrefs.HasKey("password") && _rememberPassword)
                PasswordInput.text = PlayerPrefs.GetString("password");

            LoginManager.onSuccessfulLogin += LoadMainMenu;
            LoginManager.onFailedLogin += FailedLogin;
            LoginManager.onSuccessfulAddUser += SuccessfulRegister;
            LoginManager.onFailedAddUser += FailedAddUser;
        }

        private void OnDestroy()
        {
            LoginManager.onSuccessfulLogin -= LoadMainMenu;
            LoginManager.onFailedLogin -= FailedLogin;
            LoginManager.onSuccessfulAddUser -= SuccessfulRegister;
            LoginManager.onFailedAddUser -= FailedAddUser;
        }

        #region Buttons

        private void Update()
        {
            var condition = GameControl.Client.Connected
                            && UsernameInput.text.Length >= 2 && PasswordInput.text.Length >= 2
                            && Rsa.Key.Exponent != null && Rsa.Key.Modulus != null;

            LoginButton.interactable = condition;
            AddUserButton.interactable = condition;
            OfflineButton.interactable = UsernameInput.text.Length >= 2;

            // Maybe add an option to try to reconnect
            if (!GameControl.Client.Connected)
                Debug.Log("not connected");
        }

        public void OfflineMode()
        {
            ConnectingScreen("Starting...");
            LoadMainMenu();
        }

        public void Cancel()
        {
            LoginScreen("Runling Login", "Please enter Username and Password!", Color.white);
        }

        public void RememberPasswordToggle()
        {
            _rememberPassword = RememberPassword.isOn;
            PlayerPrefs.SetInt("RememberPassword", _rememberPassword ? 1 : 0);
        }

        public void LoginButtonFunction()
        {
            PlayerPrefs.SetString("username", UsernameInput.text);
            if (_rememberPassword)
                PlayerPrefs.SetString("password", PasswordInput.text);

            ConnectingScreen("Connecting...");
            LoginManager.Login(UsernameInput.text, PasswordInput.text);
        }

        public void AddUser()
        {
            ConnectingScreen("Creating User...");
            LoginManager.AddUser(UsernameInput.text, PasswordInput.text);
        }
        #endregion


        #region Network Callback

        private void FailedLogin(byte reason)
        {
            if (reason == 1)
            {
                PasswordInput.text = "";
                LoginScreen("Login Failed!","Username/Password Combination unknown. Make sure you enter the right username and password.", Color.red);
            }
            else
            {
                LoginScreen("Server couldn't process Information", "This shouldn't happen. Please try again and let us know if the problem persists!", Color.red);
            }
        }

        private void FailedAddUser(byte reason)
        {
            if (reason == 1)
            {
                LoginScreen("Failed to create user!", "Username already taken. Please choose a different one!", Color.red);
            }
            else
            {
                LoginScreen("Server couldn't process Information", "This shouldn't happen. Please try again and let us know if the problem persists!", Color.red);
            }
        }

        private void SuccessfulRegister()
        {
            LoginButtonFunction();
        }
        #endregion

        private void ConnectingScreen(string connectionText)
        {
            ControlPanel.SetActive(false);
            CancelButton.SetActive(true);
            ConnectionText.text = connectionText;

            if (!LoadingAnimation.Particles.activeSelf)
                LoadingAnimation.StartLoaderAnimation();
        }

        private void LoginScreen(string text1, string text2, Color textColor)
        {
            ConnectionText.text = "";
            FeedbackText1.text = text1;
            FeedbackText1.color = textColor;
            FeedbackText2.text = text2;
            LoadingAnimation.StopLoaderAnimation();
            CancelButton.SetActive(false);
            ControlPanel.SetActive(true);
        }

        private static void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}