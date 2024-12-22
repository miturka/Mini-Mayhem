using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // Required for TextMeshPro

public class ToggleHoverTextChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI firstTextField;  // Reference to the first TextMeshProUGUI component
    [SerializeField] private TextMeshProUGUI secondTextField; // Reference to the second TextMeshProUGUI component
    [SerializeField] private string firstHoverText = "First Hovered Text";
    [SerializeField] private string secondHoverText = "Second Hovered Text";

    private string firstDefaultText;
    private string secondDefaultText;

    void Start()
    {
        if (firstTextField == null || secondTextField == null)
        {
            Debug.LogError("Text fields are not assigned. Please assign them in the Inspector.");
            return;
        }

        // Save the default text for both fields
        firstDefaultText = firstTextField.text;
        secondDefaultText = secondTextField.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (firstTextField != null)
        {
            firstTextField.text = firstHoverText; // Change the first text field on hover
        }
        if (secondTextField != null)
        {
            secondTextField.text = secondHoverText; // Change the second text field on hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (firstTextField != null)
        {
            firstTextField.text = firstDefaultText; // Revert the first text field to default
        }
        if (secondTextField != null)
        {
            secondTextField.text = secondDefaultText; // Revert the second text field to default
        }
    }
}
