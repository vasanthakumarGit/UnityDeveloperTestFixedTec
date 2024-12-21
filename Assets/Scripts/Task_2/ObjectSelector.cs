using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectSelector : MonoBehaviour
{
    public GameObject popupUI;
    public TMP_InputField nameField;
    public TMP_InputField positionField;
    public Button saveButton;

    private RootDetails rootDetails;
    private GameObject selectedObject;
    private JsonManager jsonManager;

    void Start()
    {
        jsonManager = GetComponent<JsonManager>();
        rootDetails = jsonManager.LoadJson();
        popupUI.SetActive(false);

        saveButton.onClick.AddListener(SavePopupDetails);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                selectedObject = hit.collider.gameObject;
                ShowPopup(selectedObject);
            }
        }
    }

    void ShowPopup(GameObject selected)
    {
        popupUI.SetActive(true);
        HighlightComponent existingComponent = GetHighlightComponent(selected.name);

        if (existingComponent != null)
        {
            nameField.text = existingComponent.name;
            positionField.text = existingComponent.position;
        }
        else
        {
            nameField.text = selected.name;
            positionField.text = Vector3ToString(selected.transform.position);
        }
    }

    HighlightComponent GetHighlightComponent(string name)
    {
        foreach (var model in rootDetails.RootDetail)
        {
            foreach (var component in model.highlight_component)
            {
                if (component.name == name)
                {
                    return component;
                }
            }
        }
        return null;
    }

    void SavePopupDetails()
    {
        string name = nameField.text;
        string position = positionField.text;

        HighlightComponent component = GetHighlightComponent(name);
        if (component == null)
        {
            component = new HighlightComponent
            {
                name = name,
                id = GenerateUniqueId(),
                position = position
            };
            rootDetails.RootDetail[0].highlight_component.Add(component); // Assumes only 1 model for simplicity
        }
        else
        {
            component.position = position;
        }

        jsonManager.SaveJson(rootDetails);
        popupUI.SetActive(false);
    }

    int GenerateUniqueId()
    {
        return Random.Range(1, 1000); // Replace with a better ID generation method if needed
    }

    string Vector3ToString(Vector3 vector)
    {
        return $"{vector.x},{vector.y},{vector.z}";
    }

    public void OnExportButtonClick()
    {
        jsonManager.ExportJson(rootDetails);
    }
}
