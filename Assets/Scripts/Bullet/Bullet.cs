using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1.0f;

    public void Start()
    {
        Destroy(gameObject,1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      

        if( collision.gameObject.GetComponent<StatManager>() )
        {
           
            collision.gameObject.GetComponent<StatManager>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
