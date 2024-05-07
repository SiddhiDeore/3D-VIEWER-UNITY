using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text;

[System.Serializable]
public class LoginResponse
{
    public bool valid;
    public string message;
}

public class LoginSystem : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TextMeshProUGUI errorMessageText;
    public LoginButton loginButtonScript;

    private string baseUrl = "http://127.0.0.1:5000";  // Make sure this is your server's address

    void Start()
    {
        loginButton.interactable = false;
        usernameInput.onValueChanged.AddListener(delegate { ValidateCredentials(); });
        passwordInput.onValueChanged.AddListener(delegate { ValidateCredentials(); });
        loginButton.onClick.AddListener(() => StartCoroutine(Login()));
    }

    void ValidateCredentials()
    {
        bool bothFieldsNotEmpty = !string.IsNullOrEmpty(usernameInput.text) && !string.IsNullOrEmpty(passwordInput.text);
        loginButton.interactable = bothFieldsNotEmpty;
        errorMessageText.gameObject.SetActive(false);
    }

    IEnumerator Login()
    {
        string url = baseUrl + "/validate-login";
        var request = new UnityWebRequest(url, "POST");
        string json = JsonUtility.ToJson(new UserCredentials { username = usernameInput.text, password = passwordInput.text });
        byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            if (response.valid)
            {
                Debug.Log("Login successful: " + response.message);
                // Proceed to next scene or enable further user interaction
                loginButtonScript.OnLoginButtonClick();
            }
            else
            {
                errorMessageText.text = "Invalid username or password";
                errorMessageText.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Error in login request: " + request.error);
            errorMessageText.text = "Error connecting to login server";
            errorMessageText.gameObject.SetActive(true);
        }
    }
}

[System.Serializable]
public class UserCredentials
{
    public string username;
    public string password;
}
