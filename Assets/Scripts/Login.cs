using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Collections;

public class Login : MonoBehaviour
{
    // Cached references
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button registerButtonFromLogin;
    public Player playerManager;
    public GameObject loginMenu;
    public GameObject registerMenu;
    public GameObject loggedInMenu;

    private string httpServerAddress;

    private void Start()
    {
        playerManager = FindObjectOfType<Player>();
        httpServerAddress = playerManager.GetHttpServer();
        if (!string.IsNullOrEmpty(playerManager.Token))
        {
            loginMenu.SetActive(false);
            loggedInMenu.SetActive(true);
        }
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(TryLogin());
    }

    /*
    private void GetToken()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/Token", "POST");

        WWWForm sendData = new WWWForm();
        sendData.AddField("grant_type", "password");
        sendData.AddField("username", emailInputField.text);
        sendData.AddField("password", passwordInputField.text);

        httpClient.uploadHandler = new UploadHandlerRaw(sendData.data);
        httpClient.downloadHandler = new DownloadHandlerBuffer();

        httpClient.SetRequestHeader("Accept", "application/json");
        httpClient.certificateHandler = new ByPassCertificate();
        httpClient.SendWebRequest();

        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log("OnGetTokenError: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            Token authToken = JsonUtility.FromJson<Token>(jsonResponse);
            playerManager.Token = authToken.access_token;
        }
        httpClient.Dispose();
    }*/

    private IEnumerator TryLogin()
    {
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);
        yield return Helper.GetPlayerInfo();
        loginMenu.SetActive(false);
        loggedInMenu.SetActive(true);
    }

    public void OnRegisterButtonClicked()
    {
        loginMenu.SetActive(false);
        registerMenu.SetActive(true);
    }

}