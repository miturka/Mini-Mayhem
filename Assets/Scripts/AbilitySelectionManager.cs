using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    
    public ToggleManager ToggleManager1; // Assign the first ToggleManager in the Inspector
    public ToggleManager ToggleManager2; // Assign the second ToggleManager in the Inspector



    void Start()
    {
        // Automatically find all ToggleManager instances in the scene
        ToggleManager[] toggleManagers = FindObjectsOfType<ToggleManager>();

        if (toggleManagers.Length >= 2)
        {
            // Assign the first two found ToggleManagers
            ToggleManager1 = toggleManagers[0];
            ToggleManager2 = toggleManagers[1];
        }
        else
        {
            Debug.LogError("Not enough ToggleManagers found in the scene!");
        }
    }


    public void ConfirmSelection()
    {
        if (ToggleManager1 != null && ToggleManager2 != null)
        {
            // Get selected abilities from both managers
            List<string> player1Abilities = ToggleManager1.GetSelectedAbilityNames();
            List<string> player2Abilities = ToggleManager2.GetSelectedAbilityNames();

            // Ensure there are at least two abilities selected for each player
            string p1PrimaryName = player1Abilities.Count > 0 ? player1Abilities[0] : "None";
            string p1SecondaryName = player1Abilities.Count > 1 ? player1Abilities[1] : "None";

            string p2PrimaryName = player2Abilities.Count > 0 ? player2Abilities[0] : "None";
            string p2SecondaryName = player2Abilities.Count > 1 ? player2Abilities[1] : "None";

            // Save ability names to PlayerPrefs
            PlayerPrefs.SetString("Player1Primary", p1PrimaryName);
            PlayerPrefs.SetString("Player1Secondary", p1SecondaryName);
            PlayerPrefs.SetString("Player2Primary", p2PrimaryName);
            PlayerPrefs.SetString("Player2Secondary", p2SecondaryName);

            // Save changes
            PlayerPrefs.Save();

            Debug.Log($"Saved Player 1: Primary = {p1PrimaryName}, Secondary = {p1SecondaryName}");
            Debug.Log($"Saved Player 2: Primary = {p2PrimaryName}, Secondary = {p2SecondaryName}");

            // Load the game scene
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogError("ToggleManager references are missing!");
        }

    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("ControlsScene"); // Prechod na scénu nastavení
    }
}
