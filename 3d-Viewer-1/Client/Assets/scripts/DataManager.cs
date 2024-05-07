using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

public class DataManager : MonoBehaviour
{
    static string serverURL = "http://127.0.0.1:5000"; // Update with your Flask server URL
    
    public static IEnumerator UpdateData(IDictionary data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        var request = new UnityWebRequest(serverURL + "/update_data", "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating data: " + request.error);
        }
        else
        {
            Debug.Log("Data updated successfully");
        }
    }

    public static IEnumerator GetData(System.Action<Dictionary<string, object>> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverURL + "/get_data"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error retrieving data: " + request.error);
            }
            else
            {
                string jsonData = request.downloadHandler.text;
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
                callback?.Invoke(data);
            }
        }
    }
}
