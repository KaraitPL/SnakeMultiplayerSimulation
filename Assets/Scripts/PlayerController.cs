using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<bool> seekTarget = new(false, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    Vector3 wayPoint;
    [SerializeField] float turnSpeed = 200;
    [SerializeField] float speed = 0;

    SnakeHeadView snakeHeadView;
    //private bool seekTarget = false;


    private void Awake()
    {
        snakeHeadView = GetComponent<SnakeHeadView>();
        SetNewDestination();
    }

    public override void OnNetworkSpawn()
    {
        transform.position = new Vector3(Random.RandomRange(-5, 5), Random.RandomRange(-5, 5), 0);
    }

    private void Update()
    {
        if (snakeHeadView.hasTarget == true && IsServer)
            seekTarget.Value = true;
        SnakeMovement();
    }

    private void SnakeMovement()
    {

        if (seekTarget.Value == true)
        {
            //Debug.Log("Seek");
            Vector3 destination = snakeHeadView.targetPosition.Value - transform.position;
            if (destination.magnitude < 0.5)
            {
                if (IsServer)
                    seekTarget.Value = false;
                destination = Vector3.zero;
            }
            Vector3 direction = snakeHeadView.targetPosition.Value - transform.position;
            float angle1 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float maxDegreesDelta = 1000f;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle1);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta * Time.deltaTime);

            destination.Normalize();
            transform.position += destination * speed * Time.deltaTime;
        }
        else
        {
            //Debug.Log("Default");
            Vector3 destination = wayPoint - transform.position;
            if (destination.magnitude < 0.3)
            {
                SetNewDestination();
                //Debug.Log("New Destination");
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
    }


    void SetNewDestination()
    {
        wayPoint = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);

    }
}