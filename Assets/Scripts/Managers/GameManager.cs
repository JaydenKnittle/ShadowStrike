using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int totalRounds = 3;
    public float roundTime = 99f;
    public float roundStartDelay = 3f;
    public float roundEndDelay = 2f;

    [Header("Current Match")]
    public int player1Wins = 0;
    public int player2Wins = 0;
    public int currentRound = 1;
    public float currentRoundTime;
    public bool roundActive = false;

    [Header("References")]
    public HealthController player1Health;
    public HealthController player2Health;

    public static event System.Action<int> OnRoundStart;
    public static event System.Action<int, bool> OnRoundEnd;
    public static event System.Action<int> OnMatchEnd;
    public static event System.Action<float> OnTimerUpdate;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(StartRound());
    }

    private void Update()
    {
        if (!roundActive) return;

        currentRoundTime -= Time.deltaTime;
        OnTimerUpdate?.Invoke(currentRoundTime);

        if (currentRoundTime <= 0)
            EndRound(DetermineWinnerByHealth());
    }

    private System.Collections.IEnumerator StartRound()
    {
        roundActive = false;
        currentRoundTime = roundTime;

        player1Health?.ResetHealth();
        player2Health?.ResetHealth();

        OnRoundStart?.Invoke(currentRound);

        yield return new WaitForSeconds(roundStartDelay);
        roundActive = true;
    }

    public void OnPlayerDied(int playerIndex)
    {
        if (!roundActive) return;
        EndRound(playerIndex == 1 ? 2 : 1);
    }

    private void EndRound(int winnerIndex)
    {
        roundActive = false;

        if (winnerIndex == 1) player1Wins++;
        else if (winnerIndex == 2) player2Wins++;

        int winsNeeded = Mathf.CeilToInt(totalRounds / 2f) + 1;
        OnRoundEnd?.Invoke(winnerIndex, false);

        if (player1Wins >= winsNeeded || player2Wins >= winsNeeded)
        {
            StartCoroutine(EndMatch(winnerIndex));
        }
        else
        {
            currentRound++;
            StartCoroutine(NextRound());
        }
    }

    private System.Collections.IEnumerator NextRound()
    {
        yield return new WaitForSeconds(roundEndDelay);
        StartCoroutine(StartRound());
    }

    private System.Collections.IEnumerator EndMatch(int winnerIndex)
    {
        yield return new WaitForSeconds(roundEndDelay);
        OnMatchEnd?.Invoke(winnerIndex);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("ArcadeVictory");
    }

    private int DetermineWinnerByHealth()
    {
        if (player1Health == null || player2Health == null) return 0;
        if (player1Health.currentHealth > player2Health.currentHealth) return 1;
        if (player2Health.currentHealth > player1Health.currentHealth) return 2;
        return 0;
    }

    public void ReturnToMainMenu() => SceneManager.LoadScene("MainMenu");
    public void RestartMatch() => SceneManager.LoadScene("Fight");
}