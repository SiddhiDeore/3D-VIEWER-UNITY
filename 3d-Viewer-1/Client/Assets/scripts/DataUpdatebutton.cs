using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUpdatebutton : MonoBehaviour
{
    // Call DataManager.Instance.UpdateData to send data to the server
    public void UpdateDataToServer()
    {
        
        StartCoroutine(DataManager.UpdateData(GameObjectPropertiesManager.modelProperties));
        GameObjectPropertiesManager.SaveEditedProperties();
    }
}
