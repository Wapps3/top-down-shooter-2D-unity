using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CotcSdk;

using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public GameObject CanvasLogin;

    public Button LoginAnonymousButton;
    public Button LoginCrendentialButton;

    public InputField Email;
    public InputField Password;
   
    public Text MessageText;

    private Cloud Cloud;
    private Gamer CurrentGamer;

    private string gamerID;
    private string gamerSecret;

    // Start is called before the first frame update
    void Start()
    {
        //Link Button to function
        LoginAnonymousButton.onClick.AddListener(LoginAnonymous);
        LoginCrendentialButton.onClick.AddListener(LoginCredential);
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
                CurrentGamer = gamer;
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
        MessageText.text = "";
        
        if (Password.text != "")
        {
            var cotc = FindObjectOfType<CotcGameObject>();

            cotc.GetCloud().Done(cloud => {
                cloud.Login(
                    network: "email",
                    networkId: Email.text,
                    networkSecret: Password.text)
                .Done(gamer => {
                    CurrentGamer = gamer;
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
            MessageText.text = "Password must be filled";
        }
        
    }

    void LaunchGame()
    {
        CanvasLogin.SetActive(false);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }

    public Gamer GetGamer()
    {
        return CurrentGamer;
    }
}
