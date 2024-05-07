using UnityEngine;
using TMPro; // Include the TextMeshPro namespace

public class DisplaySelectedOption : MonoBehaviour
{
    public TextMeshProUGUI optionText; // Assign this in the Inspector

    void Start()
    {
        // Retrieve and display the selected option
        string selectedOption = PlayerPrefs.GetString("SelectedOption", "None");
        optionText.text = "Selected Option: " + selectedOption;
    }
}
