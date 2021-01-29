using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CotcSdk;

using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public GameObject CotcSdk;

    public Button PlayButton;
    public Button LoginButton;

    public InputField Email;
    public InputField Password;
   
    public Text MessageText;

    private Cloud Cloud;

    private Gamer currentGamer;
    public Gamer CurrentGamer { get { return currentGamer; } }

    private string gamerID;
    private string gamerSecret;

    private AsyncOperation sceneAsync;

    //UI info player
    public Image playerInfomartion;

    public Text displayName;
    public Text fisrtName;
    public Text emailPlayer;

    public Image avatar;

    public Button fisrtNameEditButton;
    public Button emailEditButton;


    // Start is called before the first frame update
    void Start()
    {
        //Link Button to function
        PlayButton.onClick.AddListener(Play);
        LoginButton.onClick.AddListener(Login);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Play()
    {
        //Test to know if a player is connected or launch in guest mode
        if (currentGamer == null)
        { 
            var cotc = FindObjectOfType<CotcGameObject>();

            cotc.GetCloud().Done(cloud =>
            {
                cloud.LoginAnonymously()
                .Done(gamer => {
                    currentGamer = gamer;
                    Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                    Debug.Log("Login data: " + gamer);
                    Debug.Log("Server time: " + gamer["servertime"]);
                }, ex => {
                        // The exception should always be CotcException
                        CotcException error = (CotcException)ex;
                    Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
                });
            });
        }

        StartCoroutine(LoadGameScene());

    }

    void Login()
    {
        MessageText.text = "";

        if (Password.text == "")
        {
            MessageText.text = "Password must be filled";
            return;
        }

        StartCoroutine(WaitLogin());
    }

    IEnumerator WaitLogin()
    {
        bool connectionEstablish = false;

        var cotc = FindObjectOfType<CotcGameObject>();

        cotc.GetCloud().Done(cloud => {
            cloud.Login(
                network: "email",
                networkId: Email.text,
                networkSecret: Password.text)
            .Done(gamer => {
                currentGamer = gamer;
                Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                Debug.Log("Login data: " + gamer);
                Debug.Log("Server time: " + gamer["servertime"]);

                connectionEstablish = true;
            }, ex => {
                    // The exception should always be CotcException
                    CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });

        //While the connection is not establish continue to wait
        while (!connectionEstablish)
        {
            yield return null;
        }

        SetPlayerInformation();
    }


    void SetPlayerInformation()
    {
       
        if(currentGamer == null)
        {
            Debug.Log("Impossible to set the player information, he is null");
            return;
        }

        currentGamer.Profile.Get()
        .Done(profileRes => {
            emailPlayer.text = profileRes["email"];
            displayName.text = profileRes["displayname"];
            fisrtName.text = profileRes["firstName"];
        
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
           
        });

        playerInfomartion.gameObject.SetActive(true);
    }

    IEnumerator LoadGameScene()
    {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the GameObject which hold information about the player and his connection
        SceneManager.MoveGameObjectToScene(CotcSdk, SceneManager.GetSceneByName("GameScene"));
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("GameScene"));

        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
