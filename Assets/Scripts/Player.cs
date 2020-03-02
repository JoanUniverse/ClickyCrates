using UnityEngine;

public class Player : MonoBehaviour
{
    private const string httpServer = "https://localhost:44353";
    public string GetHttpServer()
    {
        return httpServer;
    }

    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    private string _playerId;
    public string PlayerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }

    private string _email;
    public string Email
    {
        get { return _email; }
        set { _email = value; }
    }
    private string _firstName;

    public string FirstName
    {
        get { return _firstName; }
        set { _firstName = value; }
    }

    private string _lastName;

    public string LastName
    {
        get { return _lastName; }
        set { _lastName = value; }
    }

    private string _city;

    public string City
    {
        get { return _city; }
        set { _city = value; }
    }

    private string _nickname;

    public string Nickname
    {
        get { return _nickname; }
        set { _nickname = value; }
    }

    private void Awake()
    {
        var gameManagers = FindObjectsOfType<Player>();
        if (gameManagers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}