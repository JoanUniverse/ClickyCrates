using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyGames : MonoBehaviour
{
    public Text optionText;
    public Player player;
    public Text playerText;
    public Text scoreText;
    public Text difficultyText;
    public Text durationText;
    public Text dateText;
    public GameObject myProfileMenu;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetMyLastGames());
    }

    private IEnumerator GetMyLastGames()
    {
        Player player = FindObjectOfType<Player>();

        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Game/GetMyLastGames", "POST");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Content-type", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Games > GetMyLastGames: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"games\":" + jsonResponse + "}";
            ListOfGames listOfGames = JsonUtility.FromJson<ListOfGames>(response);
            optionText.text = "My last games:";
            playerText.text = "";
            scoreText.text = "";
            difficultyText.text = "";
            durationText.text = "";
            dateText.text = "";
            foreach (GameModel g in listOfGames.games)
            {
                string userName = g.PlayerId.Substring(0, 6);
                playerText.text += "\n" + userName;
                scoreText.text += "\n" + g.Score.ToString();
                difficultyText.text += "\n" + g.Difficulty.ToString();
                durationText.text += "\n" + g.Duration;
                dateText.text += "\n" + DateTime.Parse(g.WhenPlayed);
            }

        }

        httpClient.Dispose();
    }

    private IEnumerator GetMyTopGames()
    {
        Player player = FindObjectOfType<Player>();

        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Game/GetMyTopGames", "POST");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Content-type", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Games > GetMyTopGames: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"games\":" + jsonResponse + "}";
            ListOfGames listOfGames = JsonUtility.FromJson<ListOfGames>(response);
            optionText.text = "My best games:";
            playerText.text = "";
            scoreText.text = "";
            difficultyText.text = "";
            durationText.text = "";
            dateText.text = "";
            foreach (GameModel g in listOfGames.games)
            {
                string userName = g.PlayerId.Substring(0, 6);
                playerText.text += "\n" + userName;
                scoreText.text += "\n" + g.Score.ToString();
                difficultyText.text += "\n" + g.Difficulty.ToString();
                durationText.text += "\n" + g.Duration;
                dateText.text += "\n" + DateTime.Parse(g.WhenPlayed);
            }

        }
        httpClient.Dispose();
    }

    public void OnMyLastGamesButtonClicked()
    {
        StartCoroutine(GetMyLastGames());
    }

    public void OnMyTopGamesButtonClicked()
    {
        StartCoroutine(GetMyTopGames());
    }

    public void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
        myProfileMenu.SetActive(true);
    }
}
