using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsMenuUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject statsPanel;

    [Header("Buttons (+)")]
    [SerializeField] private Button plusLife;
    [SerializeField] private Button plusSpeed;
    [SerializeField] private Button plusAttack;

    [Header("Texts (values in empty boxes)")]
    [SerializeField] private TMP_Text lifeValueText;
    [SerializeField] private TMP_Text speedValueText;
    [SerializeField] private TMP_Text attackValueText;
    [SerializeField] private TMP_Text xpValueText;

    [Header("Refs (optional - will auto-find if empty)")]
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerProgression progression;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    [Header("Behaviour")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool isOpen;

    private void Awake()
    {
        // Safety: panel must exist
        if (statsPanel != null)
            statsPanel.SetActive(false);

        // Hook button listeners (only if assigned)
        if (plusLife != null) plusLife.onClick.AddListener(OnPlusLife);
        if (plusSpeed != null) plusSpeed.onClick.AddListener(OnPlusSpeed);
        if (plusAttack != null) plusAttack.onClick.AddListener(OnPlusAttack);
    }

    private void Start()
    {
        // Auto-find refs if not assigned in inspector
        if (player == null)
            player = Object.FindFirstObjectByType<PlayerController>();

        if (progression == null && player != null)
            progression = player.GetComponent<PlayerProgression>();

        // Subscribe to changes
        if (progression != null)
            progression.OnChanged += Refresh;

        // Initial refresh if panel is open (usually closed)
        Refresh();
    }

    private void OnDestroy()
    {
        if (progression != null)
            progression.OnChanged -= Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Debug.Log("[StatsMenuUI] Toggle key pressed");
            Toggle();
        }
    }

    private void Toggle()
    {
        isOpen = !isOpen;

        if (statsPanel == null)
        {
            Debug.LogWarning("[StatsMenuUI] statsPanel is not assigned!");
            return;
        }

        statsPanel.SetActive(isOpen);

        if (pauseGameWhenOpen)
            Time.timeScale = isOpen ? 0f : 1f;

        if (isOpen)
            Refresh();
    }

    private void Refresh()
    {
        if (progression == null)
        {
            // Try again in case objects spawned later
            if (player == null)
                player = Object.FindFirstObjectByType<PlayerController>();
            if (player != null)
                progression = player.GetComponent<PlayerProgression>();

            if (progression == null) return;
        }

        if (xpValueText != null) xpValueText.text = progression.XpPoints.ToString();
        if (lifeValueText != null) lifeValueText.text = progression.bonusLife.ToString();
        if (speedValueText != null) speedValueText.text = progression.bonusSpeed.ToString();
        if (attackValueText != null) attackValueText.text = progression.bonusAttack.ToString();

        bool canSpend = progression.XpPoints > 0;

        if (plusLife != null) plusLife.interactable = canSpend;
        if (plusSpeed != null) plusSpeed.interactable = canSpend;
        if (plusAttack != null) plusAttack.interactable = canSpend;
    }

    public void OnPlusLife()
    {
        if (!EnsureRefs()) return;
        if (!progression.TrySpendPointOnLife()) return;

        player.ApplyProgression();
        Refresh();
    }

    public void OnPlusSpeed()
    {
        if (!EnsureRefs()) return;
        if (!progression.TrySpendPointOnSpeed()) return;

        player.ApplyProgression();
        Refresh();
    }

    public void OnPlusAttack()
    {
        if (!EnsureRefs()) return;
        if (!progression.TrySpendPointOnAttack()) return;

        player.ApplyProgression();
        Refresh();
    }

    private bool EnsureRefs()
    {
        if (player == null)
            player = Object.FindFirstObjectByType<PlayerController>();
        if (player == null) return false;

        if (progression == null)
            progression = player.GetComponent<PlayerProgression>();
        if (progression == null) return false;

        return true;
    }
}
