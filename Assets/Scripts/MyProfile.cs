using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyProfile : MonoBehaviour
{
    public Text playerName;
    public InputField firstNameInputField;
    public InputField lastNameInputField;
    public InputField nickNameInputField;
    public InputField cityInputField;
    public GameObject loggedInMenu;
    public GameObject myGamesMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private IEnumerator UpdatePlayer()
    {
        Player player = FindObjectOfType<Player>();
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.PlayerId;
        playerSerializable.FirstName = player.FirstName;
        playerSerializable.LastName = player.LastName;
        playerSerializable.Email = player.Email;
        playerSerializable.NickName = player.NickName;
        playerSerializable.City = player.City;

        if (!string.IsNullOrEmpty(firstNameInputField.text.Trim())) playerSerializable.FirstName = firstNameInputField.text;
        if (!string.IsNullOrEmpty(lastNameInputField.text.Trim())) playerSerializable.LastName = lastNameInputField.text;
        if (!string.IsNullOrEmpty(nickNameInputField.text.Trim())) playerSerializable.NickName = nickNameInputField.text;
        if (!string.IsNullOrEmpty(cityInputField.text.Trim())) playerSerializable.City = cityInputField.text;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.GetHttpServer() + "/api/Player/UpdatePlayer", "POST"))
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
                throw new Exception("UpdatePlayer: " + httpClient.error);
            }

            firstNameInputField.text = "";
            lastNameInputField.text = "";
            nickNameInputField.text = "";
            cityInputField.text = "";
            StartCoroutine(Helper.GetPlayerInfo());
            yield return new WaitForSeconds(1);
            playerName.text = player.NickName;
        }
    }

    public void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
        loggedInMenu.SetActive(true);
    }

    public void OnUpdateButtonClick()
    {
        StartCoroutine(UpdatePlayer());
    }

    public void OnMyGamesButtonClicked()
    {
        gameObject.SetActive(false);
        myGamesMenu.SetActive(true);
    }
}
