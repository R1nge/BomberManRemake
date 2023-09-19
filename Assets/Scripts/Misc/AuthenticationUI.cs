using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Misc
{
    public class AuthenticationUI : MonoBehaviour
    {
        [SerializeField] private GameObject loginUI, registerUI;
        [SerializeField] private TMP_InputField loginName, loginPassword;
        [SerializeField] private TMP_InputField registerName, registerEmail, registerPassword;
        [SerializeField] private Button login, register, openLogin, openRegister;
        private PlayFabManager _playFabManager;

        [Inject]
        private void Inject(PlayFabManager playFabManager) => _playFabManager = playFabManager;

        private void Awake()
        {
            openLogin.onClick.AddListener(() =>
            {
                loginUI.SetActive(true);
                registerUI.SetActive(false);
            });

            openRegister.onClick.AddListener(() =>
            {
                loginUI.SetActive(false);
                registerUI.SetActive(true);
            });

            loginName.onEndEdit.AddListener(username =>
            {
                _playFabManager.SetUserName(username);
            });

            loginPassword.onEndEdit.AddListener(password =>
            {
                _playFabManager.SetPassword(password);
            });

            registerName.onEndEdit.AddListener(username =>
            {
                _playFabManager.SetUserName(username);
            });

            registerEmail.onEndEdit.AddListener(email =>
            {
                _playFabManager.SetEmail(email);
            });

            registerPassword.onEndEdit.AddListener(password =>
            {
                _playFabManager.SetPassword(password);
            });

            login.onClick.AddListener(() =>
            {
                _playFabManager.Login();
            });

            register.onClick.AddListener(() =>
            {
                _playFabManager.Register();
            });
        }
    }
}