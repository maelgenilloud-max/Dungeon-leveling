using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene"); // <-- mets ici le NOM EXACT de ta scène de jeu
    }
}
