using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public float Health;
    public float HealthMax;
    public long Point;

    public void TakeDamage(float damage)
    {
        if(Health - damage <= 0)
        {
            Health = 0;
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.Score(Point);

            Destroy(gameObject);
        }
        else
        {
            Health -= damage;
        }
    }
}
