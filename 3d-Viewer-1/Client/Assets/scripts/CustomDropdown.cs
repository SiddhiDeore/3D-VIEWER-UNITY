using TMPro;
using UnityEngine.Events;

public class CustomDropdown : TMP_Dropdown
{
    public UnityEvent onOpen = new UnityEvent();
    public UnityEvent onClose = new UnityEvent();

    public void OpenDropdown()
    {
        onOpen.Invoke();
        base.Show();
    }

    public void CloseDropdown()
    {
        onClose.Invoke();
        base.Hide();
    }
}
