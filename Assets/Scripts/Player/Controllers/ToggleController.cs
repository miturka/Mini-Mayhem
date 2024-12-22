using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
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
            Debug.Log($"{changedToggle.name} selected");
        }
        else
        {
            // Remove the deselected toggle from the queue
            RebuildQueueWithout(changedToggle);
            Debug.Log($"{changedToggle.name} deselected");
        }

        UpdateVisuals();
    }

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

    private void UpdateVisuals()
    {
        foreach (var toggle in toggles)
        {
            // Change the toggle's background color based on its selection state
            var backgroundImage = toggle.GetComponentInChildren<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = toggle.isOn ? farbicka : Color.white;
            }
        }
    }

    public List<string> GetSelectedAbilityNames()
    {
        List<string> abilityNames = new List<string>();
        foreach (var toggle in selectedToggles)
        {
            abilityNames.Add(toggle.name);
        }
        return abilityNames;
    }
}
