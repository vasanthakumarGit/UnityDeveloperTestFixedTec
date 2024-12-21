using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Collections;

public class ModelManager : MonoBehaviour
{
    [System.Serializable]
    public class TransformData
    {
        public string position;
        public string rotation;
        public string scale;
    }

    [System.Serializable]
    public class HighlightComponent
    {
        public string name;
        public int id;
        public string position;
    }

    [System.Serializable]
    public class Model
    {
        public string name;
        public int id;
        public TransformData transform;
        public List<HighlightComponent> highlight_component = new List<HighlightComponent>();
    }

    [System.Serializable]
    public class RootDetails
    {
        public List<Model> Model = new List<Model>();
    }

    [System.Serializable]
    public class HighlightDetails
    {
        public List<HighlightData> Model = new List<HighlightData>();

        [System.Serializable]
        public class HighlightData
        {
            public int id;
            public List<int> highlightComponentId = new List<int>();
        }
    }

    [System.Serializable]
    public class ModelDetails
    {
        public List<ModelData> Model = new List<ModelData>();

        [System.Serializable]
        public class ModelData
        {
            public string name;
            public int id;
            public TransformData transform;
        }
    }

    public GameObject popupUI;
    public TMP_InputField nameInput;
    public TMP_InputField positionInput;
    public Button saveButton;
    [SerializeField]
    private TMP_Text displayText;
    [SerializeField]
    public GameObject model;

    private string jsonPath;
    private RootDetails rootDetails;
    private HighlightComponent currentComponent;



    void Start()
    {
        jsonPath = Path.Combine(Application.dataPath, "ModelData.json");
        LoadJson();
        popupUI.SetActive(false);

        saveButton.onClick.AddListener(SaveComponentDetails);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                // Call the method to handle the click on the model component
                OnComponentSelected(clickedObject);
            }
        }
    }

    void LoadJson()
    {
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            rootDetails = JsonUtility.FromJson<RootDetails>(json);
        }
        else
        {
            // Initialize with default structure
            rootDetails = new RootDetails();
            Model defaultModel = new Model
            {
                name = model.name,
                id = 0,
                transform = new TransformData
                {
                    position = "" + model.transform.position,
                    rotation = "" + model.transform.rotation.eulerAngles,
                    scale = "" + model.transform.localScale
                },
                highlight_component = new List<HighlightComponent>
                {
                    //new HighlightComponent { name = "comp1", id = 13, position = "1,1,1" },
                    //new HighlightComponent { name = "comp2", id = 12, position = "0,-1,-1" }
                }
            };
            rootDetails.Model.Add(defaultModel);

            SaveJson(); // Save the default structure to file
        }
    }


    void SaveJson()
    {
        string json = JsonUtility.ToJson(rootDetails, true);
        File.WriteAllText(jsonPath, json);
        StartCoroutine(ShowMessageForSeconds("The data has been saved successfully.", 3f));
    }

    public void OnComponentSelected(GameObject component)
    {
        string componentName = component.name;
        Debug.Log("The selected component name is : " + componentName);
        Vector3 position = component.transform.position;

        currentComponent = rootDetails.Model[0].highlight_component.Find(c => c.name == componentName);

        if (currentComponent == null)
        {
            currentComponent = new HighlightComponent { name = componentName, id = rootDetails.Model[0].highlight_component.Count + 1, position = position.ToString() };
            rootDetails.Model[0].highlight_component.Add(currentComponent);
        }

        nameInput.text = currentComponent.name;
        positionInput.text = currentComponent.position;
        popupUI.SetActive(true);
    }



    void SaveComponentDetails()
    {
        currentComponent.name = nameInput.text;
        currentComponent.position = positionInput.text;
        SaveJson();
        popupUI.SetActive(false);
    }

    public void ExportJson()
    {
        HighlightDetails highlightDetails = new HighlightDetails();
        ModelDetails modelDetails = new ModelDetails();

        foreach (var model in rootDetails.Model)
        {
            HighlightDetails.HighlightData highlightData = new HighlightDetails.HighlightData
            {
                id = model.id
            };

            foreach (var comp in model.highlight_component)
            {
                highlightData.highlightComponentId.Add(comp.id);
            }

            highlightDetails.Model.Add(highlightData);

            ModelDetails.ModelData modelData = new ModelDetails.ModelData
            {
                name = model.name,
                id = model.id,
                transform = model.transform
            };

            modelDetails.Model.Add(modelData);
        }

        string highlightJson = JsonUtility.ToJson(highlightDetails, true);
        string modelJson = JsonUtility.ToJson(modelDetails, true);

        File.WriteAllText(Path.Combine(Application.dataPath, "HighlightDetails.json"), highlightJson);
        File.WriteAllText(Path.Combine(Application.dataPath, "ModelDetails.json"), modelJson);
        StartCoroutine(ShowMessageForSeconds("The files have been exported successfully.", 2f));
    }


    private IEnumerator ShowMessageForSeconds(string message, float delay)
    {
        displayText.text = message;
        displayText.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        displayText.gameObject.SetActive(false);
    }

}
