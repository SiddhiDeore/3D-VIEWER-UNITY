using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TouchableObject : MonoBehaviour
{
    public TMP_Text textBox; 
    public GameObject[] objects; 

    private void Update()
    {
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;

            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (GameObject obj in objects)
                {
                    if (hit.collider.gameObject == obj)
                    {
                        textBox.text = obj.name;
                        break;
                    }
                }
            }
        }
    }
}
