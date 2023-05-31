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
    private float speed = 2;
    private float currentSpeed = 2f;
    private float slowSpeed;

    bool updateSize = true;

    //private bool disableSnakeView = false;
    
    private void Awake()
    {
        ratView = GetComponent<RatView>();
        SetNewDestination();
    }

    public override void OnNetworkSpawn()
    {
        transform.position = new Vector3(Random.Range(-20, 20), Random.Range(-10, 10), 0);
        snakeSeen.Value = false;
    }

    private void Update()
    {
        currentSpeed = 2 / (transform.localScale.x * transform.localScale.x);
        slowSpeed = currentSpeed / 2;


        if (ratView.hasTarget == true && IsServer)
        {
            Debug.Log("Cos serwer robi");
            snakeSeen.Value = true;
        }



        RatMovement();
        if (updateSize == true)
        {
            updateSize = false;
            StartCoroutine(MakeSmaller());
        }
    }

    void RatMovement()
    {
        Transform transform = GetComponent<Transform>();
        Vector3 rayOriginFront = transform.position + transform.right * 0.45f * transform.localScale.x;
        for (float angle = -15; angle <= 15; angle++)
        {
            Vector3 rayDirectionFront = Quaternion.Euler(0, 0, angle) * transform.right;
              
            Debug.DrawRay(rayOriginFront, rayDirectionFront * 0.8f, new Color(255, 0, 0, 1f));
            RaycastHit2D hit = Physics2D.Raycast(rayOriginFront, rayDirectionFront, 0.8f, LayerMask.GetMask("RatLayer"));

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Rat")
                {
                    speed = slowSpeed;
                }
                else
                    speed = currentSpeed;
            }
            else
                speed = currentSpeed;

            

        }
        



        if (snakeSeen.Value == true) {
            Vector3 destination = ratView.targetPosition.Value - transform.position;
            if (destination.magnitude < 0.5)
            {
                if (IsServer)
                {
                    snakeSeen.Value = false;
                    SetNewDestination();
                }
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
    IEnumerator MakeSmaller()
    {
        yield return new WaitForSeconds(2);
        float size = transform.localScale.x;
        if (size > 0.75f)
        {
            size -= 0.05f;
            transform.localScale = new Vector3(size, size, transform.localScale.z);
        }
        updateSize = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Border")
        {
            //Debug.Log("Elo");
            //disableSnakeView = true;
            if (IsServer)
                snakeSeen.Value = false;

        }
    }




}
