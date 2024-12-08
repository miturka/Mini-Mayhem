using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    public GameObject player1; // Reference to Player 1
    public GameObject player2; // Reference to Player 2

    public Image p1HealthBar;
    public Image p2HealthBar;

    public Vector3 player1SpawnPosition = new Vector3(-2, 0, 2);
    public Vector3 player2SpawnPosition = new Vector3(2, 0, -2);

    public Vector3 player1SpawnRotation = new Vector3(0, 135, 0);
    public Vector3 player2SpawnRotation = new Vector3(0, -45, 0);

    [Header("Player1 Controls")]
    public KeyCode p1UpKey = KeyCode.W;
    public KeyCode p1DownKey = KeyCode.S;
    public KeyCode p1LeftKey = KeyCode.A;
    public KeyCode p1RightKey = KeyCode.D;
    public KeyCode p1JumpKey = KeyCode.Space;
    public KeyCode p1DodgeKey = KeyCode.LeftShift;
    public KeyCode p1PrimaryAbilityKey = KeyCode.Q;
    public KeyCode p1SecondaryAbilityKey = KeyCode.E;

    [Header("Player2 Controls")]
    public KeyCode p2UpKey = KeyCode.W;
    public KeyCode p2DownKey = KeyCode.S;
    public KeyCode p2LeftKey = KeyCode.A;
    public KeyCode p2RightKey = KeyCode.D;
    public KeyCode p2JumpKey = KeyCode.Space;
    public KeyCode p2DodgeKey = KeyCode.LeftShift;
    public KeyCode p2PrimaryAbilityKey = KeyCode.Q;
    public KeyCode p2SecondaryAbilityKey = KeyCode.E;

    public GameObject playerPrefab;

    public float gameDuration = 180f; // Game timer in seconds (3 minutes)

    private float remainingTime;
    private bool isGameActive = false;

    public TMPro.TextMeshProUGUI timerText; // UI text for the timer
    public GameObject gameOverPanel; // UI panel for showing game over
    public TMPro.TextMeshProUGUI gameOverText; // UI text for game over message

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        string p1PrimaryName = PlayerPrefs.GetString("Player1Primary", null);
        string p1SecondaryName = PlayerPrefs.GetString("Player1Secondary", null);
        string p2PrimaryName = PlayerPrefs.GetString("Player2Primary", null);
        string p2SecondaryName = PlayerPrefs.GetString("Player2Secondary", null);

        StartNewGame(p1PrimaryName, p1SecondaryName, p2PrimaryName, p2SecondaryName);
    }

    public void StartNewGame(String p1PrimaryName, String p1SecondaryName, String p2PrimaryName, String p2SecondaryName)
    {
        // Reset the game state
        isGameActive = true;
        remainingTime = gameDuration;

        // Commented out spawning logic; assume players are already in the scene
        
        GameObject sceneContent = GameObject.Find("SceneContent");

        if (sceneContent == null)
        {
            Debug.LogError("SceneContent object not found in the scene.");
            return;
        }

        // Spawn hráčov
        player1 = Instantiate(playerPrefab, player1SpawnPosition, Quaternion.Euler(player1SpawnRotation));
        player2 = Instantiate(playerPrefab, player2SpawnPosition, Quaternion.Euler(player2SpawnRotation));

        // Nastavíme ich ako child objektov SceneContent
        player1.transform.SetParent(sceneContent.transform);
        player2.transform.SetParent(sceneContent.transform);

        Player player1Script = player1.GetComponent<Player>();
        Player player2Script = player2.GetComponent<Player>();

        player1Script.Initialize(p1HealthBar);
        player2Script.Initialize(p2HealthBar);

        player1Script.AddAbility(p1PrimaryName, true);
        player1Script.AddAbility(p1SecondaryName, false);
        player2Script.AddAbility(p2PrimaryName, true);
        player2Script.AddAbility(p2SecondaryName, false);

        PlayerMovement player1Movement = player1.GetComponent<PlayerMovement>();
        PlayerMovement player2Movement = player2.GetComponent<PlayerMovement>();
        AbilityController p1AbilityController = player1.GetComponent<AbilityController>();
        AbilityController p2AbilityController = player2.GetComponent<AbilityController>();

        player2Movement.upKey = p2UpKey;
        player2Movement.downKey = p2DownKey;
        player2Movement.leftKey = p2LeftKey;
        player2Movement.rightKey = p2RightKey;
        player2Movement.jumpKey = p2JumpKey;
        player2Movement.dodgeKey = p2DodgeKey;
        p2AbilityController.primaryAbilityKey = p2PrimaryAbilityKey;
        p2AbilityController.secondaryAbilityKey = p2SecondaryAbilityKey;
        
        

        if (player1 == null || player2 == null)
        {
            Debug.LogError("Players are not assigned in the GameLogic script.");
            return;
        }

        // Reset player names for reference
        player1.name = "Player 1";
        player2.name = "Player 2";

        // Hide game over UI
        gameOverPanel.SetActive(false);

        // Optionally reset player states here (e.g., health, position, abilities)
        ResetPlayers();
    }

    void Update()
    {
        if (isGameActive)
        {
            // Update the game timer
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();

            if (remainingTime <= 0)
            {
                EndGame("Time's up! It's a draw.");
            }
        }
    }

    public void PlayerDied(GameObject deadPlayer)
    {
        if (!isGameActive) return;

        string winner = deadPlayer == player1 ? "Player 2" : "Player 1";
        EndGame($"{winner} wins!");
    }

    private void EndGame(string resultMessage)
    {
        isGameActive = false;

        // Show game over UI
        gameOverPanel.SetActive(true);
        gameOverText.text = resultMessage;

        // Optionally, reload the scene after a delay
        //StartCoroutine(ReloadSceneAfterDelay(3f));
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetPlayers()
    {
        // Example: Reset health for both players
        HealthManager health1 = player1.GetComponent<HealthManager>();
        HealthManager health2 = player2.GetComponent<HealthManager>();

        //if (health1 != null) health1.ResetHealth();
        //if (health2 != null) health2.ResetHealth();
    }

    public Transform GetOpponent(GameObject currentPlayer)
    {
        if (currentPlayer == player1)
        {
            return player2.transform; // Ak je volajúci hráč player1, vráti player2
        }
        else if (currentPlayer == player2)
        {
            return player1.transform; // Ak je volajúci hráč player2, vráti player1
        }

        Debug.LogWarning("Current player not found in GameLogic!");
        return null; // Ak hráč nie je ani jeden z nich
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MenuSelect");
    }

}
