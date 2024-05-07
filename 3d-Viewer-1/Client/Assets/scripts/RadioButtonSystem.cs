using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class RadioButtonSystem : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public GameObject nextPagePanel;
    public GameObject OptionPanel; // Reference to the next page panel containing the toggle group

    private Dictionary<Toggle, string> toggleOptionMap = new Dictionary<Toggle, string>();
    private string selectedOption;

    void Start()
    {
        // Populate the dictionary with Toggle buttons and their corresponding options
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggleOptionMap.Add(toggle, toggle.name);
        }
    }

    public void OnToggleClicked(Toggle toggle)
    {
        if (toggle.isOn)
        {
            selectedOption = toggleOptionMap[toggle];
        }
    }

    public void OnNextButtonClicked()
    {
        if (!string.IsNullOrEmpty(selectedOption))
        {
            // Store the selected option
            PlayerPrefs.SetString("SelectedOption", selectedOption);

            // Load the Camera Module scene
            SceneManager.LoadScene("Free-Camera-Tanmay");
        }
        else
        {
            Debug.LogWarning("No option selected.");
        }
    }
}
