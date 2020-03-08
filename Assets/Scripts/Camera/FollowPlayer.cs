using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float speedLerp;
    public Transform playerToFollow;
    private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        offset = playerToFollow.position - gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, playerToFollow.position - offset, Time.deltaTime * speedLerp);
    }
}
