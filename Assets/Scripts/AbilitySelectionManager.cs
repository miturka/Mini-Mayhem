using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    // UI prvky pre výber schopností
    public TMP_Dropdown player1PrimaryDropdown;
    public TMP_Dropdown player1SecondaryDropdown;
    public TMP_Dropdown player2PrimaryDropdown;
    public TMP_Dropdown player2SecondaryDropdown;

    private List<string> allAbilityNames = new List<string>()
    {
        "FireMissile",
        "RapidFire",
        "Shockwave",
        "SpinAttack",
        "GrappleAndSlam",
        "DuelBreaker",
        "ChronoFlurry",
        "BlightZone"

    };

    void Start()
    {
        Debug.Log($"Populating dropdown with {allAbilityNames.Count} abilities.");

        // Naplnenie dropdownov schopnosťami
        PopulateDropdown(player1PrimaryDropdown);
        PopulateDropdown(player1SecondaryDropdown);
        PopulateDropdown(player2PrimaryDropdown);
        PopulateDropdown(player2SecondaryDropdown);
        player1PrimaryDropdown.value = 0;
        player1SecondaryDropdown.value = 1;
        player2PrimaryDropdown.value = 2;
        player2SecondaryDropdown.value = 3; 
    }

    void PopulateDropdown(TMP_Dropdown dropdown)
    {
        dropdown.AddOptions(allAbilityNames);
    }

    public void ConfirmSelection()
    {
        string p1PrimaryName = player1PrimaryDropdown.options[player1PrimaryDropdown.value].text;
        string p1SecondaryName = player1SecondaryDropdown.options[player1SecondaryDropdown.value].text;
        string p2PrimaryName = player2PrimaryDropdown.options[player2PrimaryDropdown.value].text;
        string p2SecondaryName = player2PrimaryDropdown.options[player2SecondaryDropdown.value].text;

        // Uloženie názvov do PlayerPrefs
        PlayerPrefs.SetString("Player1Primary", p1PrimaryName);
        PlayerPrefs.SetString("Player1Secondary", p1SecondaryName);
        PlayerPrefs.SetString("Player2Primary", p2PrimaryName);
        PlayerPrefs.SetString("Player2Secondary", p2SecondaryName);

        // Uloženie zmien
        PlayerPrefs.Save();

        // Prepneme na hernú scénu
        SceneManager.LoadScene("GameScene");
    }
}
