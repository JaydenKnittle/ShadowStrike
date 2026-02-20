using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RoundUIManager : MonoBehaviour
{
    [Header("Health Bars")]
    public Slider player1HealthBar;
    public Slider player2HealthBar;

    [Header("Special Meters")]
    public Slider player1SpecialMeter;
    public Slider player2SpecialMeter;

    [Header("Round Info")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI centerText;

    [Header("Win Indicators")]
    public GameObject[] player1WinDots;
    public GameObject[] player2WinDots;

    [Header("References")]
    public HealthController player1Health;
    public HealthController player2Health;
    public CombatController player1Combat;
    public CombatController player2Combat;

    private void OnEnable()
    {
        GameManager.OnRoundStart += HandleRoundStart;
        GameManager.OnRoundEnd += HandleRoundEnd;
        GameManager.OnTimerUpdate += HandleTimerUpdate;
        GameManager.OnMatchEnd += HandleMatchEnd;

        if (player1Health != null) player1Health.OnHealthChanged += UpdatePlayer1Health;
        if (player2Health != null) player2Health.OnHealthChanged += UpdatePlayer2Health;
    }

    private void OnDisable()
    {
        GameManager.OnRoundStart -= HandleRoundStart;
        GameManager.OnRoundEnd -= HandleRoundEnd;
        GameManager.OnTimerUpdate -= HandleTimerUpdate;
        GameManager.OnMatchEnd -= HandleMatchEnd;

        if (player1Health != null) player1Health.OnHealthChanged -= UpdatePlayer1Health;
        if (player2Health != null) player2Health.OnHealthChanged -= UpdatePlayer2Health;
    }

    private void Update()
    {
        if (player1Combat != null && player1SpecialMeter != null)
            player1SpecialMeter.value = player1Combat.GetSpecialMeterPercent();
        if (player2Combat != null && player2SpecialMeter != null)
            player2SpecialMeter.value = player2Combat.GetSpecialMeterPercent();
    }

    private void UpdatePlayer1Health(float current, float max)
    {
        if (player1HealthBar != null)
            player1HealthBar.value = current / max;
    }

    private void UpdatePlayer2Health(float current, float max)
    {
        if (player2HealthBar != null)
            player2HealthBar.value = current / max;
    }

    private void HandleRoundStart(int round)
    {
        if (roundText != null) roundText.text = $"Round {round}";
        StartCoroutine(ShowCenterText("FIGHT!", 1.5f));
    }

    private void HandleRoundEnd(int winner, bool isMatchEnd)
    {
        string text = winner == 0 ? "DRAW!" : $"Player {winner} Wins!";
        StartCoroutine(ShowCenterText(text, 2f));
        UpdateWinDots();
    }

    private void HandleMatchEnd(int winner)
    {
        StartCoroutine(ShowCenterText($"Player {winner} Wins the Match!", 3f));
    }

    private void HandleTimerUpdate(float time)
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(time).ToString();
    }

    private IEnumerator ShowCenterText(string text, float duration)
    {
        if (centerText == null) yield break;
        centerText.text = text;
        centerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        centerText.gameObject.SetActive(false);
    }

    private void UpdateWinDots()
    {
        if (GameManager.Instance == null) return;

        for (int i = 0; i < player1WinDots.Length; i++)
            player1WinDots[i].SetActive(i < GameManager.Instance.player1Wins);

        for (int i = 0; i < player2WinDots.Length; i++)
            player2WinDots[i].SetActive(i < GameManager.Instance.player2Wins);
    }
}