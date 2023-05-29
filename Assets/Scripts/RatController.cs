using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RatController : NetworkBehaviour
{
    public NetworkVariable<bool> snakeSeen = new(false, NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server);
    Vector3 wayPoint;
    RatView ratView;
    [SerializeField] float turnSpeed = 200;
    [SerializeField] float speed = 2;
    
    private void Awake()
    {
        ratView = GetComponent<RatView>();
        SetNewDestination();
    }

    public override void OnNetworkSpawn()
    {
        transform.position = new Vector3(Random.Range(-20, 20), Random.Range(-10, 10), 0);
    }

    private void Update()
    {
        if (ratView.hasTarget == true && IsServer)
            snakeSeen.Value = true;
        RatMovement();
    }

    void RatMovement()
    {
        Vector3 rayOriginFront = transform.position + transform.right * 0.45f;
        for (float angle = -15; angle <= 15; angle++)
        {
            Vector3 rayDirectionFront = Quaternion.Euler(0, 0, angle) * transform.right;

            Debug.DrawRay(rayOriginFront, rayDirectionFront * 0.8f, new Color(255, 0, 0, 1f));
            RaycastHit2D hit = Physics2D.Raycast(rayOriginFront, rayDirectionFront, 0.8f, LayerMask.GetMask("RatLayer"));

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Rat")
                {
                    speed = 1f;
                }
                else
                    speed = 2f;
            }
            else
                speed = 2f;

        }
        



        if (snakeSeen.Value == true) {
            Vector3 destination = ratView.targetPosition.Value - transform.position;
            if (destination.magnitude < 0.5)
            {
                if (IsServer)
                    snakeSeen.Value = false;
                //destination = Vector3.zero;
            }
            Vector3 direction = ratView.targetPosition.Value - transform.position;
            float angle1 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float maxDegreesDelta = 1000f;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle1);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta * Time.deltaTime);

            destination.Normalize();
            transform.position += destination * speed * Time.deltaTime;
        }
        else
        {
            Vector3 destination;
            Vector3 direction;
            
            destination = wayPoint - transform.position;
            if (destination.magnitude < 0.5)
            {
                SetNewDestination();
            }
            direction = wayPoint - transform.position;

            

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
    public void SetNewDestination()
    {
        wayPoint = new Vector3(Random.Range(-20, 20), Random.Range(-10, 10), 0);

    }

}
