using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoggedIn : MonoBehaviour
{
    public Text playerName;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        playerName.text = player.NickName;
    }

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(1);
    }
}
