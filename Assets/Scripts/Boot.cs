using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    private void Start() => SceneManager.LoadSceneAsync("Authentication", LoadSceneMode.Single);
}