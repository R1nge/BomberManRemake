using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    private void Start() => SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
}