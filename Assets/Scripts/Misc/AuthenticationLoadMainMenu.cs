using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Misc
{
    public class AuthenticationLoadMainMenu : MonoBehaviour
    {
        private SaveManager _saveManager;

        [Inject]
        private void Inject(SaveManager saveManager) => _saveManager = saveManager;

        private void Awake() => _saveManager.OnSaveLoaded += LoadMainMenu;

        private void LoadMainMenu() => SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);

        private void OnDestroy() => _saveManager.OnSaveLoaded -= LoadMainMenu;
    }
}