using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";

    void Awake()
    {
        // Si on n'est pas déjà dans le menu, on charge le menu
        if (SceneManager.GetActiveScene().name != menuSceneName)
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
