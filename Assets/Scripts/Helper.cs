﻿using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Helper : MonoBehaviour
{

    internal static IEnumerator InitializeToken(string email, string password)
    {
        Player player = FindObjectOfType<Player>();
        if (string.IsNullOrEmpty(player.Token))
        {
            UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/Token", "POST");

            // application/x-www-form-urlencoded
            WWWForm dataToSend = new WWWForm();
            dataToSend.AddField("grant_type", "password");
            dataToSend.AddField("username", email);
            dataToSend.AddField("password", password);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Accept", "application/json");
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Helper > InitToken: " + httpClient.error);
            }
            else
            {
                string jsonResponse = httpClient.downloadHandler.text;
                Token authToken = JsonUtility.FromJson<Token>(jsonResponse);
                player.Token = authToken.access_token;
            }
            httpClient.Dispose();
        }
    }

    internal static IEnumerator GetPlayerId()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Account/UserId", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerId: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = jsonResponse.Replace("\"", "");
            player.PlayerId = response;
        }

        httpClient.Dispose();
    }

    internal static IEnumerator GetPlayerInfo()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Player/GetPlayerInfo", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerInfo: " + httpClient.error);
        }
        else
        {
            PlayerSerializable playerSerializable = JsonUtility.FromJson<PlayerSerializable>(httpClient.downloadHandler.text);
            player.PlayerId = playerSerializable.Id;
            player.FirstName = playerSerializable.FirstName;
            player.LastName = playerSerializable.LastName;
            player.Nickname = playerSerializable.Nickname;
            player.City = playerSerializable.City;
            player.Email = playerSerializable.Email;
        }

        httpClient.Dispose();
    }

    internal static IEnumerator NewPlayerOnline()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Player/NewPlayerOnline", "POST"); ;

        string jsonData = JsonUtility.ToJson(player);
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
        httpClient.SetRequestHeader("Content-Type", "application/json");
        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);

        httpClient.certificateHandler = new ByPassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("OnNewPlayerOnline: Error > " + httpClient.error);
        }

        httpClient.Dispose();
    }

}