using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CotcSdk;

using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public Button loginAnonymousButton;
    public Button loginCrendentialButton;

    public InputField email;
    public InputField password;
   
    public Text messageText;

    private Cloud Cloud;
    private string gamerID;
    private string gamerSecret;

    // Start is called before the first frame update
    void Start()
    {
        //Link Button to function
        loginAnonymousButton.onClick.AddListener(LoginAnonymous);
        loginCrendentialButton.onClick.AddListener(LoginCredential);

        // Link with the CotC Game Object
        var cb = FindObjectOfType<CotcGameObject>();
        if (cb == null)
        {
            Debug.LogError("Please put a Clan of the Cloud prefab in your scene!");
            return;
        }
        // Log unhandled exceptions (.Done block without .Catch -- not called if there is any .Then)
        Promise.UnhandledException += (object sender, ExceptionEventArgs e) => {
            Debug.LogError("Unhandled exception: " + e.Exception.ToString());
        };
        // Initiate getting the main Cloud object
        cb.GetCloud().Done(cloud => {
            Cloud = cloud;
            // Retry failed HTTP requests once
            Cloud.HttpRequestFailedHandler = (HttpRequestFailedEventArgs e) => {
                if (e.UserData == null)
                {
                    e.UserData = new object();
                    e.RetryIn(1000);
                }
                else
                    e.Abort();
            };
            Debug.Log("Setup done");
        });
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void LoginAnonymous()
    {
        var cotc = FindObjectOfType<CotcGameObject>();

        cotc.GetCloud().Done(cloud => 
        {
            cloud.LoginAnonymously()
            .Done(gamer => {
                Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                Debug.Log("Login data: " + gamer);
                Debug.Log("Server time: " + gamer["servertime"]);
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });

        LaunchGame();

    }

    void LoginCredential()
    {
        messageText.text = "";
        
        if (password.text != "")
        {
            Debug.Log(email.text);
            Debug.Log(password.text);

            var cotc = FindObjectOfType<CotcGameObject>();

            cotc.GetCloud().Done(cloud => {
                Cloud.Login(
                    network: "email",
                    networkId: email.text,
                    networkSecret: password.text)
                .Done(gamer => {
                    Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                    Debug.Log("Login data: " + gamer);
                    Debug.Log("Server time: " + gamer["servertime"]);
                }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                    Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
                });
            });

            LaunchGame();
        }
        else
        {
            messageText.text = "Password must be filled";
        }
        
    }

    void LaunchGame()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }
}
