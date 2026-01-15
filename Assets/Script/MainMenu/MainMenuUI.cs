using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";

    private void Awake()
    {
        Debug.Log("MainMenuController Awake on: " + gameObject.name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SCENE LOADED EVENT: " + scene.name + " mode=" + mode);
    }

    public void StartGame()
    {
        Debug.Log("StartGame clicked -> will load: " + sceneName);

        // Vérifie si la scène existe dans Build Settings
        bool found = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            Debug.Log("BuildScene[" + i + "] = " + name + " (" + path + ")");
            if (name == sceneName) found = true;
        }

        if (!found)
        {
            Debug.LogError("SCENE NOT IN BUILD SETTINGS: " + sceneName);
            return;
        }

        Debug.Log("Calling SceneManager.LoadScene(" + sceneName + ")");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        Debug.Log("LoadScene call done (if nothing happens, check Build Settings / errors)");
    }
}
