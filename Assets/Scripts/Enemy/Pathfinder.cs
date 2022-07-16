using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    List<Transform> waypoints;
    float moveSpeed;
    int waypointIndex = 0;

    void Start()
    {
        //transform.position = waypoints[waypointIndex].position;
    }

    void Update()
    {
        FollowPath();
    }

    public void SetWaypoints(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }    

    void FollowPath()
    {
        if (waypointIndex < waypoints.Count)
        {
            Vector3 targetPoisition = waypoints[waypointIndex].position;
            float delta = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPoisition, delta);

            if (transform.position == targetPoisition)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
