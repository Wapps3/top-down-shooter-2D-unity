using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public float Health;
    public float HealthMax;

    public void TakeDamage(float damage)
    {
        if(Health - damage <= 0)
        {
            Health = 0;
            Destroy(gameObject);
        }
        else
        {
            Health -= damage;
        }
    }
}
