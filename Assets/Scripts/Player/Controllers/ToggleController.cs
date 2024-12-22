using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    // Used in menu where chosing abilities to toggle between 3 abilities

    public List<Toggle> toggles; // Assign the toggles in the inspector
    public int maxSelectable = 3; // Maximum number of selectable toggles

    public Color farbicka;

    private Queue<Toggle> selectedToggles = new Queue<Toggle>();

    void Start()
    {
        // Add listeners to each toggle
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggle); });
        }
    }

    // Called whenever a toggle's value is changed
    void OnToggleValueChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            // If adding this toggle exceeds the limit
            if (selectedToggles.Count >= maxSelectable)
            {
                // Deselect the oldest toggle in the queue
                Toggle oldestToggle = selectedToggles.Dequeue();
                oldestToggle.isOn = false; // This will trigger OnToggleValueChanged for the deselected toggle
            }

            // Add the newly selected toggle to the queue
            selectedToggles.Enqueue(changedToggle);
        }
        else
        {
            // Remove the deselected toggle from the queue
            RebuildQueueWithout(changedToggle);
        }

        UpdateVisuals();
    }

    // Rebuilds the queue, excluding a toggle that has been deselected
    private void RebuildQueueWithout(Toggle toggleToRemove)
    {
        // Rebuild the queue excluding the toggle that was deselected
        Queue<Toggle> newQueue = new Queue<Toggle>();
        foreach (var toggle in selectedToggles)
        {
            if (toggle != toggleToRemove)
            {
                newQueue.Enqueue(toggle);
            }
        }
        selectedToggles = newQueue;
    }

    // Updates the visuals of all toggles to reflect their current selection state
    private void UpdateVisuals()
    {
        foreach (var toggle in toggles)
        {
            // Get the background image of the toggle and apply a color based on its state
            var backgroundImage = toggle.GetComponentInChildren<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = toggle.isOn ? farbicka : Color.white;
            }
        }
    }

    // Retrieves the names of all currently selected toggles
    public List<string> GetSelectedAbilityNames()
    {
        List<string> abilityNames = new List<string>();
        foreach (var toggle in selectedToggles)
        {
            abilityNames.Add(toggle.name); // Add the name of each selected toggle to the list
        }
        return abilityNames;
    }
}
