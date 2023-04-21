using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RatController : NetworkBehaviour
{
    Vector3 wayPoint;
    [SerializeField] float turnSpeed = 200;
    [SerializeField] float speed = 2;

    private void Awake()
    {
        SetNewDestination();
    }

    private void Update()
    {
        Vector3 destination = wayPoint - transform.position;
        if (destination.magnitude < 0.3)
        {
            SetNewDestination();
        }
        Vector3 direction = wayPoint - transform.position;
        float angle1 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float tolerance = 5f;
        float bodyAngle = transform.eulerAngles.z;

        if (Mathf.Abs(angle1 - bodyAngle) > tolerance)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle1);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, angle1);
        }

        transform.position += transform.right * speed * Time.deltaTime;
    }

    void SetNewDestination()
    {
        wayPoint = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);

    }
}
