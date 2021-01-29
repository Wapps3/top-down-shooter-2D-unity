using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CotcSdk;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private CotcGameObject CotcSdk;
    private LoginManager loginManager;

    public Text scoreText;
    public Text timeText;

    public long score;
    public float time;

    private bool end;

    // Start is called before the first frame update
    void Start()
    {
        loginManager = FindObjectOfType<LoginManager>();
        CotcSdk = FindObjectOfType<CotcGameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        scoreText.text = score.ToString();
        timeText.text = time.ToString();

        if(score < 0 )
        {
            scoreText.color = new Color(1f, 0f, 0f);
        }
        else
        {
            scoreText.color = new Color(0f, 0f, 1f);
        }

        if(time <= 0 )
        {
           if(end == false)
                EndGame();
        }
    }

    public void Score(long Point)
    {
        score += Point;
    }

    void EndGame()
    {
        Gamer currentGamer = loginManager.GetGamer();

        Debug.Log(currentGamer.GamerId);

        //Save Score
        currentGamer.Scores.Domain("private").Post(score, "TopDownShooter", ScoreOrder.HighToLow,
        "Normal Mode", false)
        .Done(postScoreRes => {
            Debug.Log("Post score: " + postScoreRes.ToString());
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });

        //Show HighScore
        currentGamer.Scores.Domain("private").BestHighScores("TopDownShooter", 10, 1)
        .Done(bestHighScoresRes => {
            foreach (var score in bestHighScoresRes)
                Debug.Log(score.Rank + ". " + score.GamerInfo["profile"]["displayName"] + ": " + score.Value);
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get best high scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });

        end = true;

        StartCoroutine(LoadEndScene());
    }

    IEnumerator LoadEndScene()
    {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("EndScene", LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(CotcSdk.gameObject, SceneManager.GetSceneByName("EndScene"));
        SceneManager.MoveGameObjectToScene(loginManager.gameObject, SceneManager.GetSceneByName("EndScene"));

        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
