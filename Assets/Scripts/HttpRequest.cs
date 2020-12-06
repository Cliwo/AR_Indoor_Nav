using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class HttpRequest : MonoBehaviour
{
    private static string OAuthToken =
        "xoxp-1535876942773-1524231484999-1550123834869-aefa1c2b3b3e5ea71e6cf799d0566601";
    public static string SeungchanJeong = "U01FE6TE8VD";
    public static string SeungchanLim = "U01FRS071S9";
    public static string HyunyoungJang = "U01FE6L3DQX";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetPresense());
        StartCoroutine(GetStatusText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetPresense()
    {
        WWWForm form = new WWWForm();
        form.AddField("user", SeungchanLim);
        UnityWebRequest www = UnityWebRequest.Post("https://slack.com/api/users.getPresence", form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer "+OAuthToken);
        yield return www.SendWebRequest();
        
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log(www.downloadHandler.text);
            JObject jObject = JObject.Parse(www.downloadHandler.text);
            Debug.Log("(Presence)Extract exact result : " + (jObject["presence"]));
        }

        yield return null;
    }
    
    IEnumerator GetStatusText()
    {
        WWWForm form = new WWWForm();
        form.AddField("user", SeungchanJeong);
        UnityWebRequest www = UnityWebRequest.Post("https://slack.com/api/users.info", form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Bearer "+OAuthToken);
        yield return www.SendWebRequest();
        
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log(www.downloadHandler.text);
            JObject jObject = JObject.Parse(www.downloadHandler.text);
            Debug.Log("(Status Text)Extract exact result : " + jObject["user"]["profile"]["status_text"]);
        }

        yield return null;
    }
}
