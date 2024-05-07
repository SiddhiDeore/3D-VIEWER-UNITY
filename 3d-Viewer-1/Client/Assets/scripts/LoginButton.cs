using UnityEngine;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject nextPagePanel;

    public void OnLoginButtonClick()
    {
        Debug.Log("Login button clicked");
        // Disable the login panel
        loginPanel.SetActive(false);

        // Enable the next page panel
        nextPagePanel.SetActive(true);
    }
}
