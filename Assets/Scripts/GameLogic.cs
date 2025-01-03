using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;
    public GameObject player1; // Reference to Player 1
    public GameObject player2; // Reference to Player 2

    public Image p1HealthBar;
    public Image p2HealthBar;

    public Image p1PrimaryCooldownBar;
    public Image p1SecondaryCooldownBar;
    public Image p1TertiaryCooldownBar;
    public Image p2PrimaryCooldownBar;
    public Image p2SecondaryCooldownBar;
    public Image p2TertiaryCooldownBar;

    public Image p1PrimaryCooldownImage;
    public Image p1SecondaryCooldownImage;
    public Image p1TertiaryCooldownImage;
    public Image p2PrimaryCooldownImage;
    public Image p2SecondaryCooldownImage;
    public Image p2TertiaryCooldownImage;
    
    public TextMeshProUGUI p1Health;
    public TextMeshProUGUI p2Health;

    public Vector3 player1SpawnPosition = new Vector3(-2, 0, 2);
    public Vector3 player2SpawnPosition = new Vector3(2, 0, -2);

    public Vector3 player1SpawnRotation = new Vector3(0, 135, 0);
    public Vector3 player2SpawnRotation = new Vector3(0, -45, 0);

    public GameObject playerPrefab;

    public float gameDuration = 180f; // Game timer in seconds (3 minutes)

    private float remainingTime;
    private bool isGameActive = false;

    public FollowCamera followCamera;

    public TextMeshProUGUI timerText; // UI text for the timer
    public GameObject gameOverPanel; // UI panel for showing game over
    public TextMeshProUGUI gameOverText; // UI text for game over message

    public GameObject pauseMenuPanel; 

    public GameObject pausePanel; // UI panel for showing game over
    private bool isGamePaused = false;

    public Dictionary<string, Sprite> abilityImageMap = new Dictionary<string, Sprite>();

    public Sprite groundSlam;
    public Sprite blightZone;
    public Sprite chronoFlurry;
    public Sprite duelBreaker;
    public Sprite fireMissile;
    public Sprite rapidFire;
    public Sprite shockwave;
    public Sprite quickJab;

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
        // Get ability names from menuselect screen
        abilityImageMap["GroundSlam"] = groundSlam;
        abilityImageMap["BlightZone"] = blightZone;
        abilityImageMap["ChronoFlurry"] = chronoFlurry;
        abilityImageMap["DuelBreaker"] = duelBreaker;
        abilityImageMap["FireMissile"] = fireMissile;
        abilityImageMap["RapidFire"] = rapidFire;
        abilityImageMap["Shockwave"] = shockwave;
        abilityImageMap["QuickJab"] = quickJab;

        string p1PrimaryName = PlayerPrefs.GetString("Player1Primary", null);
        string p1SecondaryName = PlayerPrefs.GetString("Player1Secondary", null);
        string p1TertiaryName = PlayerPrefs.GetString("Player1Tertiary", null);
        string p2PrimaryName = PlayerPrefs.GetString("Player2Primary", null);
        string p2SecondaryName = PlayerPrefs.GetString("Player2Secondary", null);
        string p2TertiaryName = PlayerPrefs.GetString("Player2Tertiary", null);

        // start new game with the abilities
        StartNewGame(p1PrimaryName, p1SecondaryName, p1TertiaryName, p2PrimaryName, p2SecondaryName, p2TertiaryName);

        pauseMenuPanel.SetActive(false);
    }

    public void StartNewGame(String p1PrimaryName, String p1SecondaryName, String p1TertiaryName, String p2PrimaryName, String p2SecondaryName, String p2TertiaryName)
    {
        // Reset the game state
        isGameActive = true;
        remainingTime = gameDuration;
        
        GameObject sceneContent = GameObject.Find("SceneContent");

        if (sceneContent == null)
        {
            Debug.LogError("SceneContent object not found in the scene.");
            return;
        }

        // Spawn hráčov
        player1 = Instantiate(playerPrefab, player1SpawnPosition, Quaternion.Euler(player1SpawnRotation));
        player2 = Instantiate(playerPrefab, player2SpawnPosition, Quaternion.Euler(player2SpawnRotation));

        followCamera.player1 = player1.transform;
        followCamera.player2 = player2.transform;

        // set players as child objects of SceneContent
        player1.transform.SetParent(sceneContent.transform);
        player2.transform.SetParent(sceneContent.transform);

        Player player1Script = player1.GetComponent<Player>();
        Player player2Script = player2.GetComponent<Player>();

        player2Script.changeMaterial();

        p1PrimaryCooldownImage.sprite = abilityImageMap[p1PrimaryName];
        p1SecondaryCooldownImage.sprite = abilityImageMap[p1SecondaryName];
        p1TertiaryCooldownImage.sprite = abilityImageMap[p1TertiaryName];
        p2PrimaryCooldownImage.sprite = abilityImageMap[p2PrimaryName];
        p2SecondaryCooldownImage.sprite = abilityImageMap[p2SecondaryName];
        p2TertiaryCooldownImage.sprite = abilityImageMap[p2TertiaryName];


        player1Script.Initialize(p1HealthBar, p1Health, p1PrimaryCooldownBar, p1SecondaryCooldownBar, p1TertiaryCooldownBar);
        player2Script.Initialize(p2HealthBar, p2Health, p2PrimaryCooldownBar, p2SecondaryCooldownBar, p2TertiaryCooldownBar);

        player1Script.AddAbility(p1PrimaryName, 0);
        player1Script.AddAbility(p1SecondaryName, 1);
        player1Script.AddAbility(p1TertiaryName, 2);
        player2Script.AddAbility(p2PrimaryName, 0);
        player2Script.AddAbility(p2SecondaryName, 1);
        player2Script.AddAbility(p2TertiaryName, 2);

        PlayerMovement player1Movement = player1.GetComponent<PlayerMovement>();
        PlayerMovement player2Movement = player2.GetComponent<PlayerMovement>();
        AbilityController p1AbilityController = player1.GetComponent<AbilityController>();
        AbilityController p2AbilityController = player2.GetComponent<AbilityController>();

        player1Movement.upKey = GetKeyFromPrefs("0_Move Forward", KeyCode.W);
        player1Movement.downKey = GetKeyFromPrefs("0_Move Back", KeyCode.S);
        player1Movement.leftKey = GetKeyFromPrefs("0_Move Left", KeyCode.A);
        player1Movement.rightKey = GetKeyFromPrefs("0_Move Right", KeyCode.D);
        player1Movement.jumpKey = GetKeyFromPrefs("0_Jump", KeyCode.Space);
        player1Movement.dodgeKey = GetKeyFromPrefs("0_Dodge", KeyCode.LeftShift);
        p1AbilityController.primaryAbilityKey = GetKeyFromPrefs("0_Use Ability 1", KeyCode.T);
        p1AbilityController.secondaryAbilityKey = GetKeyFromPrefs("0_Use Ability 2", KeyCode.Y);
        p1AbilityController.tertiaryAbilityKey = GetKeyFromPrefs("0_Use Ability 3", KeyCode.U);

        player2Movement.upKey = GetKeyFromPrefs("1_Move Forward", KeyCode.Keypad8);
        player2Movement.downKey = GetKeyFromPrefs("1_Move Back", KeyCode.Keypad5);
        player2Movement.leftKey = GetKeyFromPrefs("1_Move Left", KeyCode.Keypad4);
        player2Movement.rightKey = GetKeyFromPrefs("1_Move Right", KeyCode.Keypad6);
        player2Movement.jumpKey = GetKeyFromPrefs("1_Jump", KeyCode.Keypad0);
        player2Movement.dodgeKey = GetKeyFromPrefs("1_Dodge", KeyCode.KeypadEnter);
        p2AbilityController.primaryAbilityKey = GetKeyFromPrefs("1_Use Ability 1", KeyCode.Delete);
        p2AbilityController.secondaryAbilityKey = GetKeyFromPrefs("1_Use Ability 2", KeyCode.End);
        p2AbilityController.tertiaryAbilityKey = GetKeyFromPrefs("1_Use Ability 3", KeyCode.PageDown);
        

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (isGameActive)
        {
            // Update the game timer
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(remainingTime, 0);
            UpdateTimerUI();

            if (remainingTime <= 0)
            {
                Player p1Script = player1.GetComponent<Player>();
                Player p2Script = player2.GetComponent<Player>();
                string winner = "";
                if (p1Script.getHealth() > p2Script.getHealth())
                {
                    winner = "Player 1 wins!";
                }
                else if (p1Script.getHealth() < p2Script.getHealth())
                {
                    winner = "Player 2 wins!";
                }
                else
                {
                    winner = "It's a tie!";
                }
                EndGame(winner);
            }

            // Check for Escape key to toggle pause menu
            
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // Zobraziť Pause Menu
        Time.timeScale = 0f; // Zastaviť čas
        isGameActive = false; // Pauznúť hru
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Skryť Pause Menu
        Time.timeScale = 1f; // Obnoviť čas
        isGameActive = true; // Obnoviť hru
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Obnoviť čas (ak by bolo pauznuté)
        isGameActive = true;
        SceneManager.LoadScene("MainMenuScene"); // Prepnúť na hlavnú menu scénu
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
        Time.timeScale = 1f; // Obnoviť čas (ak by bolo pauznuté)
        isGameActive = true;
        SceneManager.LoadScene("MenuSelect");
    }

    KeyCode GetKeyFromPrefs(string keyName, KeyCode defaultKey)
    {
        string keyString = PlayerPrefs.GetString(keyName, defaultKey.ToString()); // Načítanie hodnoty ako string
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyString); // Konverzia string na KeyCode
    }

}
