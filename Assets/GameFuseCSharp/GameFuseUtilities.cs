﻿using System;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using UnityEngine;
using UnityEngine.Networking;


namespace GameFuseCSharp
{
    /// <summary>Class <c>GameFuseUtilities</c> is your connection with the GameFuse
    /// API.  Thorugh this class you can connect to your apps, login users,
    /// create users.  When a user is signed in you can use GameFuseUser to 
    /// access your account, attributes and purchased store items.
    /// </summary>
    public class GameFuseUtilities : MonoBehaviour
    {

        internal static void HandleCallback(UnityWebRequest request, string successString, Action<string, bool> callback = null)
        {
            Debug.Log("Recieved "+request.responseCode+" response for "+request.url);
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Request had error: " + request.downloadHandler.text);
                if (callback != null)
                    callback("An error occurred: " + request.downloadHandler.text, true);
            }
            else if (request.responseCode == 299)
            {
                var errorString = request.downloadHandler.text;
                if (errorString.Contains("has already been taken"))
                    errorString = "Username or email already taken";

                if (callback != null)
                    callback(errorString, true);
               
             }

            else
            {
                if (callback != null)
                    callback(successString, false);
            }
        }


        internal static bool RequestIsSuccessful(UnityWebRequest request)
        {
            return request.result != UnityWebRequest.Result.ProtocolError && request.result != UnityWebRequest.Result.ConnectionError && request.responseCode != 299;
        }
    }

}


