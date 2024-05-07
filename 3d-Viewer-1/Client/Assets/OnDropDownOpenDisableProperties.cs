using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDropDownOpenDisableProperties : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private void OnEnable()
    {
        //if(GameObjectPropertiesManager.instance != null)
        //{
        //    GameObjectPropertiesManager.instance.ToggleUIVisibility(false);
        //}
        
    }

    void Start()
    {
        if (name == "Dropdown List")
        {
            if (GameObjectPropertiesManager.instance != null)
            {
                GameObjectPropertiesManager.instance.ToggleUIVisibility(false);
            }
        }
    }

   
    public void OnDestroy()
    {

        if (name == "Dropdown List")
        {
            if (GameObjectPropertiesManager.instance != null)
            {
                GameObjectPropertiesManager.instance.ToggleUIVisibility(true);
            }
        }

    }

}
