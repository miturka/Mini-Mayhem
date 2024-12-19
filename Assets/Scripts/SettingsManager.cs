using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class SettingsManager : MonoBehaviour
{
    public Transform player1Panel; // Panel pre hráča 1
    public Transform player2Panel; // Panel pre hráča 2
    public GameObject keyBindPrefab; // Prefab pre text a tlačidlo

    private List<string> functions = new List<string>
    {
        "Move Forward",
        "Move Back",
        "Move Left",
        "Move Right",
        "Jump",
        "Dodge",
        "Use Ability 1",
        "Use Ability 2"
    };

    private string waitingForKeyFunction = null;
    private int waitingForKeyPlayer = -1;
    private Button waitingForKeyButton = null;

    void Start()
    {
        PopulateKeyBindings(player1Panel, 0); // Pre hráča 1
        PopulateKeyBindings(player2Panel, 1); // Pre hráča 2
    }

    void PopulateKeyBindings(Transform panel, int playerIndex)
    {
        foreach (string function in functions)
        {
            // Vytvor UI prvky
            GameObject keyBindUI = Instantiate(keyBindPrefab, panel);
            TMP_Text functionText = keyBindUI.transform.Find("FunctionText").GetComponent<TMP_Text>();
            Button keyButton = keyBindUI.transform.Find("KeyButton").GetComponent<Button>();
            TMP_Text buttonText = keyButton.GetComponentInChildren<TMP_Text>();

            // Nastav text názvu funkcie
            functionText.text = function;

            // Získaj aktuálny kláves alebo predvolený
            string currentKey = PlayerPrefs.GetString($"{playerIndex}_{function}", GetDefaultKey(function, playerIndex).ToString());
            buttonText.text = currentKey;

            // Pripoj listener na tlačidlo
            keyButton.onClick.AddListener(() => StartRebinding(function, playerIndex, keyButton));
        }
    }

    void StartRebinding(string function, int playerIndex, Button keyButton)
    {
        waitingForKeyFunction = function;
        waitingForKeyPlayer = playerIndex;
        waitingForKeyButton = keyButton;

        // Zmeň text tlačidla na "Press any key..."
        TMP_Text buttonText = keyButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "Press any key...";
    }

    void Update()
    {
        if (waitingForKeyFunction != null && Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    // Ulož nový kláves
                    PlayerPrefs.SetString($"{waitingForKeyPlayer}_{waitingForKeyFunction}", key.ToString());
                    PlayerPrefs.Save();

                    // Zmeň text tlačidla na nový kláves
                    TMP_Text buttonText = waitingForKeyButton.GetComponentInChildren<TMP_Text>();
                    buttonText.text = key.ToString();

                    // Reset rebindingu
                    waitingForKeyFunction = null;
                    waitingForKeyPlayer = -1;
                    waitingForKeyButton = null;
                    break;
                }
            }
        }
    }

    string GetDefaultKey(string function, int playerIndex)
    {
        // Definuj predvolené klávesy
        Dictionary<string, KeyCode> defaultKeysPlayer1 = new Dictionary<string, KeyCode>
        {
            { "Move Forward", KeyCode.W },
            { "Move Back", KeyCode.S },
            { "Move Left", KeyCode.A },
            { "Move Right", KeyCode.D },
            { "Jump", KeyCode.Space },
            { "Dodge", KeyCode.LeftShift },
            { "Use Ability 1", KeyCode.Q },
            { "Use Ability 2", KeyCode.E }
        };

        Dictionary<string, KeyCode> defaultKeysPlayer2 = new Dictionary<string, KeyCode>
        {
            { "Move Forward", KeyCode.Keypad8},
            { "Move Back", KeyCode.Keypad5 },
            { "Move Left", KeyCode.Keypad4 },
            { "Move Right", KeyCode.Keypad6 },
            { "Jump", KeyCode.Keypad0 },
            { "Dodge", KeyCode.KeypadEnter},
            { "Use Ability 1", KeyCode.Delete },
            { "Use Ability 2", KeyCode.Home }
        };

        return (playerIndex == 0 ? defaultKeysPlayer1 : defaultKeysPlayer2)[function].ToString();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // Prechod na scénu nastavení
    }
}
