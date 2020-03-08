using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public List<Transform> firePoint;
    private int lastIndexShoot = 0;


    public GameObject bulletPrefab;

    public float bulletForce = 20.0f;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }


    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab,firePoint[lastIndexShoot].position, firePoint[lastIndexShoot].rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint[lastIndexShoot].up * bulletForce, ForceMode2D.Impulse);

        if (lastIndexShoot == firePoint.Count - 1)
        {
            lastIndexShoot = 0;
        }
        else
        {
            lastIndexShoot++;
        }

    }
}
