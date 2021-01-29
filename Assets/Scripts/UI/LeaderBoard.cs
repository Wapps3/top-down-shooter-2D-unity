using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

using CotcSdk;

public class LeaderBoard : MonoBehaviour
{
    public Button ReplayButton;
    public Button ExitButton;

    public Text Score;

    public List<Text> LeaderBoardScore;

    private CotcGameObject CotcSdk;
    private LoginManager loginManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        ReplayButton.onClick.AddListener(Replay);
        ExitButton.onClick.AddListener(Exit);

        loginManager = FindObjectOfType<LoginManager>();
        CotcSdk = FindObjectOfType<CotcGameObject>();

        PrintLeaderBoard();
    }

    void PrintLeaderBoard()
    {
        GameManager GameManagerObject = FindObjectOfType<GameManager>();

        Score.text = (GameManagerObject.Score).ToString();

        Gamer currentGamer = loginManager.GetGamer();

        //Show HighScore
        currentGamer.Scores.Domain("private").BestHighScores("TopDownShooter", LeaderBoardScore.Count, 1)
        .Done(bestHighScoresRes => {
            int index = 0;
            foreach (var score in bestHighScoresRes)
            {
                LeaderBoardScore[index].text = (score.Rank + ". " + score.GamerInfo["profile"]["displayName"] + ": " + score.Value);
                index++;
            }
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get best high scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Replay()
    {
        StartCoroutine(LoadGameScene());
    }

    void Exit()
    {
        Application.Quit();
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
        SceneManager.MoveGameObjectToScene(CotcSdk.gameObject, SceneManager.GetSceneByName("GameScene"));
        SceneManager.MoveGameObjectToScene(loginManager.gameObject, SceneManager.GetSceneByName("GameScene"));

        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
