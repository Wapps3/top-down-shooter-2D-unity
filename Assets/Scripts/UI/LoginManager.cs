using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CotcSdk;

using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public GameObject CotcSdk;

    public Button LoginAnonymousButton;
    public Button LoginCrendentialButton;

    public InputField Email;
    public InputField Password;
   
    public Text MessageText;

    private Cloud Cloud;
    private Gamer CurrentGamer;

    private string gamerID;
    private string gamerSecret;

    private AsyncOperation sceneAsync;

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

        StartCoroutine(LoadGameScene());

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

            StartCoroutine(LoadGameScene());
        }
        else
        {
            MessageText.text = "Password must be filled";
        }
        
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

        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(CotcSdk, SceneManager.GetSceneByName("GameScene"));
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("GameScene"));


        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }

    public Gamer GetGamer()
    {
        return CurrentGamer;
    }
}
