using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public FixedJoystick moveJoystick;
    public Button upButton;
    public Button downButton;
   // public TMP_Text textBox;

    public void SetUIElementsActive(bool active)
    {
        moveJoystick.gameObject.SetActive(active);
        upButton.gameObject.SetActive(active);
        downButton.gameObject.SetActive(active);
    }
}
