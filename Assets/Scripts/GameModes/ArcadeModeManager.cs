using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ArcadeModeManager : MonoBehaviour
{
    public static ArcadeModeManager Instance;

    [Header("Arcade Settings")]
    public List<CharacterData> opponentRoster;
    public int currentOpponentIndex = 0;
    public int playerWins = 0;

    [Header("Selected Character")]
    public CharacterData selectedCharacter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public CharacterData GetCurrentOpponent()
    {
        if (currentOpponentIndex < opponentRoster.Count)
            return opponentRoster[currentOpponentIndex];
        return null;
    }

    public void OnFightWon()
    {
        playerWins++;
        currentOpponentIndex++;

        if (currentOpponentIndex >= opponentRoster.Count)
        {
            SceneManager.LoadScene("ArcadeVictory");
        }
        else
        {
            SceneManager.LoadScene("Fight");
        }
    }

    public bool IsFinalBoss() => currentOpponentIndex == opponentRoster.Count - 1;

    public void ResetArcade()
    {
        currentOpponentIndex = 0;
        playerWins = 0;
    }
}