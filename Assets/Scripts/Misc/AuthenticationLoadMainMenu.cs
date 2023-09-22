using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Misc
{
    public class AuthenticationLoadMainMenu : MonoBehaviour
    {
        private PlayFabManager _playFabManager;

        [Inject]
        private void Inject(PlayFabManager playFabManager) => _playFabManager = playFabManager;

        private void Awake() => _playFabManager.OnLoginSuccessful += LoadMainMenu;

        private void LoadMainMenu() => StartCoroutine(Wait());

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        }

        private void OnDestroy() => _playFabManager.OnLoginSuccessful -= LoadMainMenu;
    }
}