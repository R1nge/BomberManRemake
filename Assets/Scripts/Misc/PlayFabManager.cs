using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class PlayFabManager : IInitializable
    {
        public event Action OnLoginSuccessful;
        
        private string _email, _userName, _password;
        private string _userID;

        public string GetUserName => _userName;

        public string GetUserID => _userID;

        public void Initialize()
        {
            PlayFabSettings.staticSettings.TitleId = "57D27";
        }

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

        public async void Register()
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

            var registerTask = PlayFabClientAPI.RegisterPlayFabUserAsync(request);

            await registerTask;
            
            _userID = registerTask.Result.Result.PlayFabId;
            
            Debug.Log("Register successful");
            
            Login();
        }

        public async void Login()
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
            
            var loginTask = PlayFabClientAPI.LoginWithPlayFabAsync(request);

            await loginTask;
            
            _userID = loginTask.Result.Result.PlayFabId;
            OnLoginSuccessful?.Invoke();
            
            Debug.Log("Login successful");
        }
    }
}