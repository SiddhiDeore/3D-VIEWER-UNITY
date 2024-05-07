using UnityEngine;
using UnityEngine.UI; // For UI elements like Dropdown
using System.Collections.Generic; // For using Lists

public class ExportGameObjectInfo : MonoBehaviour
{
    public Dropdown gameObjectDropdown; // Assign this from the Inspector

    void Start()
    {
        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        List<string> gameObjectNames = new List<string>();
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            // Assuming you want to list all GameObjects, otherwise add your filter logic here
            gameObjectNames.Add(obj.name);
        }

        gameObjectDropdown.ClearOptions();
        gameObjectDropdown.AddOptions(gameObjectNames);

        // Optional: Add listener for dropdown value changes
        gameObjectDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(gameObjectDropdown);
        });
    }

    // Optional: Handler for when Dropdown selection changes
    void DropdownValueChanged(Dropdown change)
    {
        Debug.Log("Selected GameObject: " + change.options[change.value].text);
        // Implement any action to be taken when a new GameObject is selected from the dropdown
    }
}
