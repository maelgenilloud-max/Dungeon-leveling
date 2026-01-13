using UnityEngine;
using System.Collections; // Obligatoire pour les Coroutines
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public GameObject deathScreen;
    public Transform respawnPoint;
    public float fadeSpeed = 2f; // Vitesse du fondu

    private CanvasGroup deathScreenGroup;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // On récupère le CanvasGroup sur l'écran de mort
        deathScreenGroup = deathScreen.GetComponent<CanvasGroup>();

        deathScreen.SetActive(true); // On le laisse actif mais invisible
        deathScreenGroup.alpha = 0;
        deathScreenGroup.interactable = false; // Empêche de cliquer à travers au début
        deathScreenGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        // On lance l'apparition progressive
        StartCoroutine(FadeInDeathScreen());

        // Optionnel : On peut ralentir le temps au lieu de l'arrêter net
        Time.timeScale = 0.2f;

        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    IEnumerator FadeInDeathScreen()
    {
        while (deathScreenGroup.alpha < 1)
        {
            // On utilise "Time.unscaledDeltaTime" car le temps est presque arrêté
            deathScreenGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        deathScreenGroup.interactable = true;
        deathScreenGroup.blocksRaycasts = true;
        Time.timeScale = 0f; // On finit par arrêter le temps complètement
    }

    public void Respawn()
    {
        isDead = false;
        health = 100;
        Time.timeScale = 1f;

        if (respawnPoint != null) transform.position = respawnPoint.position;

        // Reset visuel immédiat
        deathScreenGroup.alpha = 0;
        deathScreenGroup.interactable = false;
        deathScreenGroup.blocksRaycasts = false;

        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }
}