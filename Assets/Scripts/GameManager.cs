using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject titleScreen;
    public Button restartButton;
    public Button mainMenu;
    public TextMeshProUGUI gameOverText;
    public List<GameObject> targets;
    public TextMeshProUGUI scoreText;
    public Text playersOnline;
    private int score;
    private float spawnRate = 1.0f;
    public bool isGameActive;
    private int gameDifficulty;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RefreshPlayersOnline());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = UnityEngine.Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        mainMenu.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        if(isGameActive) StartCoroutine(InsertGame());
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame(int difficulty)
    {
        gameDifficulty = difficulty;
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
        StartCoroutine(SpawnTarget());
        score = 0;
        spawnRate /= difficulty;
        UpdateScore(0);
    }

    private IEnumerator InsertGame()
    {
        Player player = FindObjectOfType<Player>();

        GameModel gameModel = new GameModel();
        gameModel.Score = score;
        gameModel.Difficulty = gameDifficulty;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Game/InsertGame", "POST"))
        {
            string gameData = JsonUtility.ToJson(gameModel);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(gameData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.downloadHandler = new DownloadHandlerBuffer();
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("InsertGame > Game: " + httpClient.error);
            }
            Debug.Log("Works");
        }
    }

    private IEnumerator GetPlayersOnline()
    {
        Player player = FindObjectOfType<Player>();

        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Player/GetPlayersOnline", "POST");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Content-type", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new ByPassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Games > GetPlayersOnline: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"players\":" + jsonResponse + "}";
            ListOfPlayers listOfPlayers = JsonUtility.FromJson<ListOfPlayers>(response);
            playersOnline.text = "";
            foreach (PlayerSerializable p in listOfPlayers.players)
            {
                string userName = p.Id.Substring(0, 4);
                playersOnline.text += "\n" + userName + "(" + p.NickName + ")";
            }
        }
    }

    private IEnumerator RefreshPlayersOnline()
    {
        while (true)
        {
            StartCoroutine(GetPlayersOnline());
            yield return new WaitForSeconds(2);
        }
    }


    public void OnMainMenuButtonClicked()
    {
        StartCoroutine(Helper.DeletePlayerOnline());
        SceneManager.LoadScene(0);
    }
}
