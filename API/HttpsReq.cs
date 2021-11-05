using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
//TODO: Cleanup

public class HttpsReq : MonoBehaviour
{
    public void GetRequest(string uri, System.Action<string> CallBack)
    {
        StartCoroutine(RawGetRequest(uri, CallBack));
    }

    private IEnumerator RawGetRequest(string uri, System.Action<string> CallBack)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                //TODO: Error callback
                //TODO: remove some logs
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    CallBack(webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}