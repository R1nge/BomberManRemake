using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Misc
{
    public class PlayFabManager
    {
        public event Action OnLoginSuccessful;
        
        private string _email, _userName, _password;
        private string _userID;

        public string GetUserName => _userName; 
        
        public string GetUserID => _userID;

        public void SetUserName(string username)
        {
            _userName = username;
        }

        public void SetEmail(string email)
        {
            _email = email;
        }

        public void SetPassword(string password)
        {
            _password = password;
        }

        public void Register()
        {
            if (_userName == string.Empty || _email == string.Empty || _password == string.Empty)
            {
                return;
            }

            var request = new RegisterPlayFabUserRequest
            {
                Username = _userName,
                Email = _email,
                Password = _password,
                DisplayName = _userName
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError);
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            _userID = result.PlayFabId;
            Debug.Log("Successful register");
        }

        private void OnRegisterError(PlayFabError error)
        {
            Debug.LogError($"Error while registering; {error.GenerateErrorReport()}");
        }

        public void Login()
        {
            if (_userName == string.Empty || _password == string.Empty)
            {
                return;
            }

            var request = new LoginWithPlayFabRequest
            {
                Username = _userName,
                Password = _password
            };

            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Successful login");
            _userID = result.PlayFabId;
            OnLoginSuccessful?.Invoke();
        }

        private void OnLoginError(PlayFabError error)
        {
            Debug.LogError($"Error while logging in; {error.GenerateErrorReport()}");
        }
    }
}