using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections;

public class GameObjectPropertiesManager : MonoBehaviour
{

    public static GameObjectPropertiesManager instance;

    public TMP_Dropdown dropdown;
    public GameObject inputFieldPrefab; // Assign a prefab of TMP_InputField in the editor
    public RectTransform propertiesPanel; // Assign a parent panel for input fields in the editor
    public Button saveButton; // Already assigned
    public Button addButton; // Already assigned
    public Button deleteButton;
    public TextAsset jsonFile; // Assign this in the Unity Editor
    public GameObject scrollView;

    public GameObject confirmationDialog; // Assign in Unity Editor for deletion confirmation
    public GameObject saveChangesDialog; // Assign in Unity Editor for save changes confirmation
    public Button okButton;

    public static Dictionary<string, Dictionary<string, string>> modelProperties;
    public static Dictionary<string, Dictionary<string, string>> originalModelProperties;
    private static string selectedModel;
    private bool isUIVisible = false;
    private bool isDeletionMode = false;
    private static bool changesMade = false;

    void Start()
    {
        instance = this;
        //LoadModelProperties();
        StartCoroutine(LoadModelProperties());
        PopulateDropdown();
        dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(dropdown.value); });        
        ToggleUIVisibility(isUIVisible);
        deleteButton.onClick.AddListener(ShowDeleteConfirmation);
        okButton.onClick.AddListener(DeleteSelectedProperties);
        okButton.gameObject.SetActive(false);
        confirmationDialog.SetActive(false);
        saveChangesDialog.SetActive(false);        
    }

    
    private void ShowDeleteConfirmation()
    {
        confirmationDialog.SetActive(true);
        Button yesButton = confirmationDialog.transform.Find("Yes").GetComponent<Button>();
        Button noButton = confirmationDialog.transform.Find("No").GetComponent<Button>();

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => {
            isDeletionMode = true; // Enable deletion mode
            deleteButton.interactable = false; // Disable the delete button
            GeneratePropertiesUI(isDeletionMode);
            okButton.gameObject.SetActive(true);
            confirmationDialog.SetActive(false);
        });

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => {
            confirmationDialog.SetActive(false);
            deleteButton.interactable = true; // Re-enable the delete button
        });
    }

    private void DeleteSelectedProperties()
    {
        List<string> keysToDelete = new List<string>();
        foreach (Transform item in propertiesPanel)
        {
            Toggle checkbox = item.Find("Checkbox").GetComponent<Toggle>();
            if (checkbox.isOn)
            {
                TMP_InputField keyField = item.Find("key").GetComponent<TMP_InputField>();
                keysToDelete.Add(keyField.text);
            }
        }

        foreach (string key in keysToDelete)
        {
            modelProperties[selectedModel].Remove(key);
        }

        //SavePropertiesToFile();  // Save immediately after deleting properties
        SavePropertiesToServer();

        isDeletionMode = false;
        ClearExistingPropertiesUI();
        GeneratePropertiesUI();  // Refresh UI without checkboxes
        okButton.gameObject.SetActive(false);
        deleteButton.interactable = true;
    }

    IEnumerator LoadModelProperties()
    {
        /*jsonFile = Resources.Load<TextAsset>("gameObjects1");
        if (jsonFile != null)
        {
            string jsonContent = jsonFile.text;
            modelProperties = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);
            originalModelProperties = new Dictionary<string, Dictionary<string, string>>(modelProperties); // Deep copy
        }
        else
        {
            Debug.LogError("JSON file not assigned in the Inspector or not found.");
        }*/

        yield return StartCoroutine(DataManager.GetData((data) =>
        {
            // Handle the retrieved data here
            if (data != null && data.ContainsKey("modelProperties"))
            {
                string jsonData = data["modelProperties"].ToString();
                modelProperties = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
                originalModelProperties = new Dictionary<string, Dictionary<string, string>>(modelProperties); // Deep copy
            }
            else
            {
                Debug.LogError("Error: Model properties data not found or invalid format.");
            }
        }));
    }

    void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> modelNamesExceptPlane = new List<string>();

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<MeshFilter>() != null && !obj.name.Equals("Plane"))
            {
                modelNamesExceptPlane.Add(obj.name);
            }
        }

        dropdown.AddOptions(modelNamesExceptPlane);
    }

    public void OnDropdownValueChanged(int index)
    {
        string newModel = dropdown.options[index].text;
        if (changesMade && selectedModel != newModel)
        {
            ShowSaveChangesDialog(() => {
                SavePropertiesToFile();
                selectedModel = newModel;
                RefreshUI();
            }, () => {
                RevertChanges();
                selectedModel = newModel;
                RefreshUI();
            });
        }
        else
        {
            selectedModel = newModel;
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        GameObject selectedGameObject = GameObject.Find(selectedModel);
        ClearExistingPropertiesUI();
        GeneratePropertiesUI();
        StartCoroutine(DelayedShowPropertyUI());
        FocusCameraOnSelectedObject(selectedGameObject);
    }

    private void FocusCameraOnSelectedObject(GameObject selectedGameObject)
    {
        CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
        if (cameraScript != null && cameraScript.CurrentController != null)
        {
            cameraScript.CurrentController.FocusOnTarget(selectedGameObject);
        }
        else
        {
            Debug.LogError("Camera script not found.");
        }
    }

    public void ToggleUI()
    {
        isUIVisible = !isUIVisible;
        Dropdowntoggle(isUIVisible);
        ToggleUIVisibility(isUIVisible);
    }

    private void Dropdowntoggle(bool isVisible)
    {
        dropdown.gameObject.SetActive(isVisible);
    }

    internal void ToggleUIVisibility(bool isVisible)
    {
        // Show or hide the UI elements related to the properties, but keep the dropdown always visible
        scrollView.SetActive(isVisible);
        // Assuming dropdown, addButton, and saveButton are meant to be toggled together
        //dropdown.gameObject.SetActive(isVisible);
        addButton.gameObject.SetActive(isVisible);
        saveButton.gameObject.SetActive(isVisible);
        deleteButton.gameObject.SetActive(isVisible);
        propertiesPanel.gameObject.SetActive(isVisible);
        // propertiesPanel visibility is managed by individual property input fields
    }


    private void RevertChanges()
    {
        if (originalModelProperties.ContainsKey(selectedModel))
        {
            modelProperties[selectedModel] = new Dictionary<string, string>(originalModelProperties[selectedModel]);
            changesMade = false;
            Debug.Log("Reverted changes for: " + selectedModel);
        }
        else
        {
            Debug.LogError("Model not found in original properties.");
        }
    }

    private void UpdateSelectedModel(int index)
    {
        selectedModel = dropdown.options[index].text;
        GameObject selectedGameObject = GameObject.Find(selectedModel);

        // Clear existing UI elements and regenerate them to reflect potentially reverted or updated data
        ClearExistingPropertiesUI();
        GeneratePropertiesUI();

        StartCoroutine(DelayedShowPropertyUI());

        // Assuming your camera script is attached to the main camera and has a method to get the current camera controller
        CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
        if (cameraScript != null && cameraScript.CurrentController != null)
        {
            cameraScript.CurrentController.FocusOnTarget(selectedGameObject);
        }
        else
        {
            Debug.LogError("Camera script not found on the main camera");
        }
    }

    private IEnumerator DelayedShowPropertyUI()
    {
        yield return new WaitForEndOfFrame();
        if (isUIVisible)
        {
            propertiesPanel.gameObject.SetActive(true);
            saveButton.gameObject.SetActive(true);
            addButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(true);
            scrollView.SetActive(true);
        }
    }

    private void ShowSaveChangesDialog(Action onYes, Action onNo)
    {
        saveChangesDialog.SetActive(true);
        Button yesButton = saveChangesDialog.transform.Find("Yes").GetComponent<Button>();
        Button noButton = saveChangesDialog.transform.Find("No").GetComponent<Button>();

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => {
            onYes.Invoke();
            saveChangesDialog.SetActive(false);
            changesMade = false; // Reset the flag
        });

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => {
            RevertChanges();  // Explicitly call RevertChanges to ensure properties are reset
            onNo.Invoke();
            saveChangesDialog.SetActive(false);
        });
    }

    private void GeneratePropertiesUI(bool withCheckboxes = false)
    {
        ClearExistingPropertiesUI();

        if (!modelProperties.ContainsKey(selectedModel)) return;

        foreach (KeyValuePair<string, string> property in modelProperties[selectedModel])
        {
            GameObject fieldObj = Instantiate(inputFieldPrefab, propertiesPanel);
            TMP_InputField keyField = fieldObj.transform.Find("key").GetComponent<TMP_InputField>();
            TMP_InputField valueField = fieldObj.transform.Find("value").GetComponent<TMP_InputField>();
            Toggle checkbox = fieldObj.transform.Find("Checkbox").GetComponent<Toggle>();

            keyField.text = property.Key;
            valueField.text = property.Value;
            checkbox.gameObject.SetActive(withCheckboxes);
            checkbox.isOn = false;

            keyField.onEndEdit.AddListener(newKey => {
                UpdatePropertyKey(property.Key, newKey);
                changesMade = true;
            });
            valueField.onEndEdit.AddListener(newValue => {
                modelProperties[selectedModel][property.Key] = newValue;
                changesMade = true;
            });
        }
    }

    private void UpdatePropertyKey(string oldKey, string newKey)
    {
        if (!modelProperties[selectedModel].ContainsKey(oldKey)) return;
        var value = modelProperties[selectedModel][oldKey];
        modelProperties[selectedModel].Remove(oldKey);
        modelProperties[selectedModel][newKey] = value;
    }


    private void ClearExistingPropertiesUI()
    {
        foreach (Transform child in propertiesPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddNewProperty()
    {
        if (!string.IsNullOrEmpty(selectedModel) && modelProperties.ContainsKey(selectedModel))
        {
            // Start with the count of properties as the next potential property number
            int maxPropNum = modelProperties[selectedModel].Count;

            // Generate the initial key based on current count
            string newPropKey = $"property{maxPropNum}";

            // Check if the key already exists and find the next available number
            while (modelProperties[selectedModel].ContainsKey(newPropKey))
            {
                newPropKey = $"property{++maxPropNum}";
            }

            // Assign a default value to the new property key
            modelProperties[selectedModel][newPropKey] = "New Value";

            // Optionally, save changes immediately (if you have this method set up to save changes)
            //SavePropertiesToFile();
            SavePropertiesToServer();

            // Clear and regenerate the properties UI to reflect the changes
            ClearExistingPropertiesUI();
            GeneratePropertiesUI();
        }
        else
        {
            Debug.LogError("Selected model is null or not found in the properties dictionary.");
        }
    }


    private void SavePropertiesToServer()
    {
        StartCoroutine(DataManager.UpdateData(modelProperties));
    }

    public static void SaveEditedProperties()
    {
        SavePropertiesToFile(); // Call the method to save the updated properties to the JSON file.
    }

    private static void SavePropertiesToFile()
    {
        string jsonToSave = JsonConvert.SerializeObject(modelProperties, Formatting.Indented);
        string filePath = Path.Combine(Application.dataPath, "Resources/gameObjects1.json");
        File.WriteAllText(filePath, jsonToSave);
        Debug.Log("Properties saved to file: " + filePath);
        originalModelProperties[selectedModel] = new Dictionary<string, string>(modelProperties[selectedModel]);
        changesMade = false;
    }
}