using System.IO;
using UnityEngine;
using System.Collections.Generic;
public class JsonManager : MonoBehaviour
{
    private string jsonPath;

    private void Awake()
    {
        jsonPath = Application.persistentDataPath + "/ModelData.json";
    }

    public void SaveJson(RootDetails rootDetails)
    {
        string json = JsonUtility.ToJson(rootDetails, true);
        File.WriteAllText(jsonPath, json);
        Debug.Log("JSON Saved: " + jsonPath);
    }

    public RootDetails LoadJson()
    {
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            Debug.Log("JSON Loaded: " + json);
            return JsonUtility.FromJson<RootDetails>(json);
        }
        return new RootDetails();
    }

    public void ExportJson(RootDetails rootDetails)
    {
        HighlightJson highlightJson = new HighlightJson();
        ModelJson modelJson = new ModelJson();

        foreach (var model in rootDetails.RootDetail)
        {
            // Populate ModelJson
            modelJson.ModelDetails.Add(new ModelDetails
            {
                name = model.name,
                id = model.id,
                transform = model.transform
            });

            // Populate HighlightJson
            HighlightDetails highlight = new HighlightDetails
            {
                id = model.id
            };
            foreach (var component in model.highlight_component)
            {
                if (component != null)
                {
                    highlight.highlightComponentId.Add(component.id);
                }
            }
            highlightJson.HighlightDetails.Add(highlight);
        }

        // Save JSON
        string highlightJsonPath = Application.persistentDataPath + "/HighlightData.json";
        string modelJsonPath = Application.persistentDataPath + "/ModelData.json";

        File.WriteAllText(highlightJsonPath, JsonUtility.ToJson(highlightJson, true));
        File.WriteAllText(modelJsonPath, JsonUtility.ToJson(modelJson, true));

        Debug.Log("JSON Exported to: " + highlightJsonPath + " and " + modelJsonPath);
    }

}


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
public class ModelDetails
{
    public string name;
    public int id;
    public TransformData transform;
    public List<HighlightComponent> highlight_component = new List<HighlightComponent>();
}

[System.Serializable]
public class RootDetails
{
    public List<ModelDetails> RootDetail = new List<ModelDetails>();
}

[System.Serializable]
public class HighlightDetails
{
    public int id;
    public List<int> highlightComponentId = new List<int>();
}

[System.Serializable]
public class HighlightJson
{
    public List<HighlightDetails> HighlightDetails = new List<HighlightDetails>();
}

[System.Serializable]
public class ModelJson
{
    public List<ModelDetails> ModelDetails = new List<ModelDetails>();
}

