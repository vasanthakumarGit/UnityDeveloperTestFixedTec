using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class JSONLoader : MonoBehaviour
{
    public TMP_Text displayNameText;
    public TMP_Text displayMessageText;
    public TMP_Text errorText;
    public GameObject loadingIndicator;
    public Button loadButton;

    void Start()
    {
        loadButton.onClick.AddListener(LoadData);
    }

    void LoadData()
    {
        loadingIndicator.SetActive(true);

        string path = Path.Combine(Application.persistentDataPath, "PIDData.json");

        if (!File.Exists(path))
        {
            loadingIndicator.SetActive(false); 
            DisplayErrorMessage("File not found.");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            Root root = JsonUtility.FromJson<Root>(json);
            DisplayData(root);
        }
        catch (System.Exception e)
        {
            loadingIndicator.SetActive(false); 
            DisplayErrorMessage($"Error parsing JSON: {e.Message}");
        }
    }

    private void DisplayData(Root root)
    {
        if (root?.results?.Length > 0 && root.results[0].codes?.Length > 0)
        {
            var firstCode = root.results[0].codes[0];
            var firstMessage = firstCode.messages.Length > 0 ? firstCode.messages[0] : null;

            if (firstMessage != null)
            {
                displayNameText.text = firstCode.short_name;
                displayMessageText.text = firstMessage.message;
            }
            else
            {
                errorText.text = "No message found for the code.";
            }
        }
        else
        {
            errorText.text = "No codes available.";
        }

        loadingIndicator.SetActive(false); 
    }

    private void DisplayErrorMessage(string message)
    {
        errorText.text = message;
    }
}

[System.Serializable]
public class Result
{
    public Code[] codes;
}

[System.Serializable]
public class Code
{
    public string short_name;
    public Message[] messages;
}

[System.Serializable]
public class Message
{
    public string code;
    public string message;
}

[System.Serializable]
public class Root
{
    public Result[] results;
}
