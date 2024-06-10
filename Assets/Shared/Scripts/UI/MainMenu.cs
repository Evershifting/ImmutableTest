using System;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Immutable.Passport;

namespace HyperCasual.Runner
{
    /// <summary>
    /// This View contains main menu functionalities
    /// </summary>
    public class MainMenu : View
    {
        [SerializeField]
        HyperCasualButton _getJWT, _sendJWT;
        [SerializeField]
        HyperCasualButton m_TryAgainButton;
        [SerializeField]
        HyperCasualButton m_ContinuePassportButton;
        [SerializeField]
        HyperCasualButton m_StartButton;
        [SerializeField]
        AbstractGameEvent m_StartButtonEvent;
        [SerializeField]
        TextMeshProUGUI m_Email;
        [SerializeField]
        HyperCasualButton m_LogoutButton;
        [SerializeField]
        GameObject m_Loading;
        Passport passport;
        async void OnEnable()
        {
            ShowLoading(true);

            // Set listener to 'Start' button
            m_StartButton.RemoveListener(OnStartButtonClick);
            m_StartButton.AddListener(OnStartButtonClick);
            // Set listener to 'Logout' button
            m_LogoutButton.RemoveListener(OnLogoutButtonClick);
            m_LogoutButton.AddListener(OnLogoutButtonClick);

            // Initialize Passport
            string clientId = "FOL3VT6Qzi3dOb5OeZBPp0p8e1an7d5H";
            string environment = Immutable.Passport.Model.Environment.SANDBOX;
            passport = await Passport.Init(clientId, environment);

            ShowLoading(false);
            ShowStartButton(true);

            // Set listener to "Continue with Passport" button
            m_ContinuePassportButton.RemoveListener(OnContinueWithPassportButtonClicked);
            m_ContinuePassportButton.AddListener(OnContinueWithPassportButtonClicked);

            _getJWT.RemoveListener(GetJWT);
            _getJWT.AddListener(GetJWT);

            _sendJWT.RemoveListener(SendJWT);
            _sendJWT.AddListener(SendJWT);
        }


        private async void GetJWT()
        {
            //get JWT token from immutable passport
            string AccessToken = await passport.GetAccessToken();
            string IdToken = await passport.GetIdToken();
            //string walletAddress = await passport.GetAddress();
            //Debug.Log($"ZZZ GetAddress: {walletAddress}");

            Debug.Log($"ZZZ GetAccessToken: {DecodeJWT(AccessToken)}");
            Debug.Log($"ZZZ GetIdToken: {DecodeJWT(IdToken)}");
            //Debug.Log($"ZZZ GetIdToken: {DecodeJWT(walletAddress)}");
        }

        string DecodeJWT(string token)
        {
            Debug.Log($"ZZZ Token: {token}");
            string[] parts = token.Split('.');
            var payload = parts[1];
            var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64));
            return json;
        }
        private void SendJWT()
        {
            //send JWT token to server
        }
        void OnDisable()
        {
            m_StartButton.RemoveListener(OnStartButtonClick);
        }

        void OnStartButtonClick()
        {
            m_StartButtonEvent.Raise();
            AudioManager.Instance.PlayEffect(SoundID.ButtonSound);
        }

        void OnLogoutButtonClick()
        {
            try
            {
                // Hide the 'Logout' button
                ShowLogoutButton(false);
                // Show loading
                ShowLoading(true);

                // Logout

                // Reset the login flag
                SaveManager.Instance.IsLoggedIn = false;
                // Successfully logged out, hide 'Logout' button
                ShowLogoutButton(false);
                // Reset all other values
                SaveManager.Instance.Clear();
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to log out: {ex.Message}");
                // Failed to logout so show 'Logout' button again
                ShowLogoutButton(true);
            }
            // Hide loading
            ShowLoading(false);
        }

        void ShowLoading(bool show)
        {
            m_Loading.gameObject.SetActive(show);
            ShowStartButton(!show);
        }

        void ShowStartButton(bool show)
        {
            m_StartButton.gameObject.SetActive(show);
        }

        void ShowLogoutButton(bool show)
        {
            m_LogoutButton.gameObject.SetActive(show);
        }

        void ShowEmail(bool show)
        {
            m_Email.gameObject.SetActive(show);
        }


        private async void OnContinueWithPassportButtonClicked()
        {

            try
            {
                // Show loading
                ShowContinueWithPassportButton(false);
                ShowLoading(true);

                // Log into Passport
                await Passport.Instance.Login();

                // Successfully logged in
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to log into Passport: {ex.Message}");
                // Show Continue with Passport button again
                ShowContinueWithPassportButton(true);
                ShowLoading(false);
            }
        }
        private void ShowContinueWithPassportButton(bool show)
        {
            m_ContinuePassportButton.gameObject.SetActive(show);
        }

    }
}