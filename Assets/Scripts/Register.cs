using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Globalization;

public class Register : MonoBehaviour
{
    // Cached references
    public InputField firstNameInputField;
    public InputField lastNameInputField;
    public InputField nicknameInputField;
    public InputField cityInputField;
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public GameObject loginMenu;
    public Player player;

    public void OnRegisterButtonClick()
    {
        StartCoroutine(RegisterNewUser());
    }

    private IEnumerator RegisterNewUser()
    {
        yield return RegisterUser();
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);  //Sets player.Token
        yield return Helper.GetPlayerId();  //Sets player.Id
        player.Email = emailInputField.text;
        player.FirstName = firstNameInputField.text;
        player.LastName = lastNameInputField.text;
        player.City = cityInputField.text;
        player.NickName = nicknameInputField.text;
        yield return InsertPlayer();
        player.PlayerId = string.Empty;
        player.Token = string.Empty;
        player.FirstName = string.Empty;
        player.Email = string.Empty;
        player.LastName = string.Empty;
    }

    private IEnumerator RegisterUser()
    {
        UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Account/Register", "POST");

        AspNetUserRegister newUser = new AspNetUserRegister();
        newUser.Email = emailInputField.text;
        newUser.Password = passwordInputField.text;
        newUser.ConfirmPassword = confirmPasswordInputField.text;

        string jsonData = JsonUtility.ToJson(newUser);
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
        httpClient.SetRequestHeader("Content-Type", "application/json");

        httpClient.certificateHandler = new ByPassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("OnRegisterButtonClick: Error > " + httpClient.error);
        }

        //messageBoardText.text += "\nOnRegisterButtonClick: " + httpClient.responseCode;

        httpClient.Dispose();
    }

    private IEnumerator InsertPlayer()
    {
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.PlayerId;
        playerSerializable.FirstName = player.FirstName;
        playerSerializable.LastName = player.LastName;
        playerSerializable.Email = player.Email;
        playerSerializable.NickName = player.NickName;
        playerSerializable.City = player.City;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Player/InsertNewPlayer", "POST"))
        {
            string playerData = JsonUtility.ToJson(playerSerializable);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.downloadHandler = new DownloadHandlerBuffer();
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new ByPassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("RegisterNewPlayer > InsertPlayer: " + httpClient.error);
            }

            OnBackToLoginMenuButtonClicked();

            //messageBoardText.text += "\nRegisterNewPlayer > InsertPlayer: " + httpClient.responseCode;
        }
    }

    public void OnBackToLoginMenuButtonClicked()
    {
        gameObject.SetActive(false);
        loginMenu.SetActive(true);
    }
}