using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CotcSdk;

public class Monster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        CotcGameObject cotcSdk = FindObjectOfType<CotcGameObject>();

        if (cotcSdk)
        { 
            LoginManager loginManager = FindObjectOfType<LoginManager>();

            Gamer currentGamer = loginManager.CurrentGamer;

            currentGamer.Achievements.Domain("private").List().Done(listAchievementsRes =>
            {

                    foreach (var achievement in listAchievementsRes)
                    {
                        if (achievement.Value.Config["unit"] == "KilledMonsters")
                        {
                            achievement.Value.Progress += 1 / achievement.Value.Config["maxValue"];
                        }
                    }

                    foreach (var achievement in listAchievementsRes)
                    {
                        Debug.Log(achievement.Key + " : " + achievement.Value.Config.ToString() + ", progress : " + achievement.Value.Progress);
                    }

                }, ex =>
                {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                    Debug.LogError("Could not list achievements: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
            
        }

    }
}
