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

    private long score;
    public long Score{ get { return score; } }

    public float time;

    private bool end;

    // Start is called before the first frame update
    void Start()
    {
        loginManager = FindObjectOfType<LoginManager>();
        CotcSdk = FindObjectOfType<CotcGameObject>();

        CreateAchievemments();
    }

    public void CreateAchievemments()
    {
        Debug.Log("Create achivements");
        
        Gamer currentGamer = loginManager.CurrentGamer;

        Bundle achievementBundle = Bundle.CreateObject();

        achievementBundle["type"] = new Bundle("limit");
        achievementBundle["progress"] = new Bundle(0f);

        Bundle config = Bundle.CreateObject();
        config["unit"] = new Bundle("KilledMonsters");
        config["maxValue"] = new Bundle(5);

        achievementBundle["config"] = config;

        currentGamer.Achievements.Domain("private").List().Done(listAchievementsRes => {

            AchievementDefinition newAchivement = new AchievementDefinition("Kill 5 Monsters", achievementBundle);
            listAchievementsRes.Add("Kill 5 Monsters", newAchivement);

            foreach (var achievement in listAchievementsRes)
            {
                Debug.Log(achievement.Key + " : " + achievement.Value.Config.ToString() + ", progress : " + achievement.Value.Progress  );
            }

        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not list achievements: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });

        Debug.Log("Finish to Create achivements");
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

    public void IncrementScore(long Point)
    {
        score += Point;
    }

    void EndGame()
    {
        Gamer currentGamer = loginManager.CurrentGamer;

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

        gameObject.SetActive(false);

        // Move the GameObject which hold information about the player and his score
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("EndScene"));
        SceneManager.MoveGameObjectToScene(CotcSdk.gameObject, SceneManager.GetSceneByName("EndScene"));
        SceneManager.MoveGameObjectToScene(loginManager.gameObject, SceneManager.GetSceneByName("EndScene"));

        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
