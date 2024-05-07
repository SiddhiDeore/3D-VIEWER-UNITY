using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Image pointerImage;

    public GameObject[] objects;
    public TMP_Text textBox;

    private Transform _selection;

    void Start()
    {
        textBox.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    foreach (GameObject obj in objects)
                    {
                        if (hit.collider.gameObject == obj)
                        {
                            TMP_Text tmpText = obj.GetComponentInChildren<TMP_Text>();
                            if (tmpText != null)
                            {
                                string dimensions = GetObjectDimensions(obj);
                                textBox.text = tmpText.text + "\n" + dimensions;
                                textBox.gameObject.SetActive(true);
                            }
                            break;
                        }
                    }

                    var selection = hit.transform;
                    if (selection.CompareTag(selectableTag))
                    {
                        ChangeMaterial(selection);
                    }
                    else
                    {
                        ResetMaterial();
                        textBox.gameObject.SetActive(false);
                    }
                }
                else
                {
                    ResetMaterial();
                    textBox.gameObject.SetActive(false);
                }
            }
        }
    }

    void ChangeMaterial(Transform selection)
    {
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material = defaultMaterial;
            }
        }

        var newSelectionRenderer = selection.GetComponent<Renderer>();
        if (newSelectionRenderer != null)
        {
            newSelectionRenderer.material = highlightMaterial;
        }
        _selection = selection;
    }

    private string GetObjectDimensions(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Collider>().bounds;

        string dimensions = "Dimensions: " + bounds.size.x.ToString("F2") + " x " + bounds.size.y.ToString("F2") + " x " + bounds.size.z.ToString("F2");

        return dimensions;
    }

    void ResetMaterial()
    {
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material = defaultMaterial;
                _selection = null;
            }
        }
    }
}
