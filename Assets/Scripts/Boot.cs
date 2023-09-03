using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    private void Awake() => SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
}