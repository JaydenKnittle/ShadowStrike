using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance;

    [Header("Available Characters")]
    public CharacterData[] availableCharacters;

    [Header("Selections")]
    public CharacterData player1Selection;
    public CharacterData player2Selection;
    public int player1Index = 0;
    public int player2Index = 1;

    [Header("Game Mode")]
    public GameMode currentGameMode;

    public enum GameMode { LocalMultiplayer, Arcade, Survival }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Player1SelectCharacter(int index)
    {
        player1Index = Mathf.Clamp(index, 0, availableCharacters.Length - 1);
        player1Selection = availableCharacters[player1Index];
    }

    public void Player2SelectCharacter(int index)
    {
        player2Index = Mathf.Clamp(index, 0, availableCharacters.Length - 1);
        player2Selection = availableCharacters[player2Index];
    }

    public void StartFight()
    {
        if (player1Selection == null || player2Selection == null)
        {
            Debug.LogWarning("Both players must select a character!");
            return;
        }
        SceneManager.LoadScene("Fight");
    }

    public void SetGameMode(GameMode mode) => currentGameMode = mode;
}