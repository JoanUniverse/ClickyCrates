using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoggedIn : MonoBehaviour
{
    public Text playerName;
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
        player = FindObjectOfType<Player>();
        playerName.text = player.NickName;
        StartCoroutine(GetLastGames());
    }

    public void OnPlayButtonClicked()
    {
        StartCoroutine(NewPlayerOnlineCommit());
        SceneManager.LoadScene(1);
    }

    public void OnMyProfileButtonClicked()
    {
        gameObject.SetActive(false);
        myProfileMenu.SetActive(true);
    }

    private IEnumerator GetLastGames()
    {
        Player player = FindObjectOfType<Player>();

        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Game/GetLastGames", "POST");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Content-type", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Games > GetLastGames: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"games\":" + jsonResponse + "}";
            ListOfGames listOfGames = JsonUtility.FromJson<ListOfGames>(response);
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
    private IEnumerator NewPlayerOnlineCommit()
    {
        yield return Helper.NewPlayerOnline();
    }
}
