using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using CotcSdk;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Text timeText;

    public long score;
    public float time;

    private bool end;

    // Start is called before the first frame update
    void Start()
    {
        end = false;
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

        LoginManager loginManager = FindObjectOfType<LoginManager>();

    }

    public void Score(long Point)
    {
        score += Point;
    }

    void EndGame()
    {
        LoginManager loginManager = FindObjectOfType<LoginManager>();

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

        SceneManager.LoadScene("EndScene", LoadSceneMode.Additive);
    }
}
