using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class RatView : NetworkBehaviour
{
    public NetworkVariable<Vector3> targetPosition = new(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Server);

    RatController controller;

    public bool hasTarget = false;
    public bool cheeseSpotted = false;
    public bool run = false;
    private bool updateSize = true;

    public bool disableSnakeView = false;

    private Transform transform;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        controller = GetComponent<RatController>();
        targetPosition.Value = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float minCheeseDistance = 10000f;
        bool targetSeen = false;
        for (int angle = -90; angle <= 90; angle++)
        {
            Vector3 ray = Quaternion.Euler(0, 0, angle) * transform.right;
            Vector3 rayTarget = transform.right;
            LayerMask mask = LayerMask.GetMask("SnakeLayer") | LayerMask.GetMask("CheeseLayer");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, 4f, mask);
            //Debug.DrawRay(transform.position, ray * 4, new Color(0, 0, 255, 0.10f));  //Rysuje radar

            RaycastHit2D hitTarget = Physics2D.Raycast(transform.position, rayTarget, 0.5f * transform.localScale.x, LayerMask.GetMask("CheeseLayer")); //Ray do zjadania sera (krótki)

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Player")
                {
                    if(run == false)
                        StartCoroutine(TurnOnRunForFewSeconds());
                    hasTarget = true;
                    targetSeen = true;
                    if (IsServer)
                    {
                        
                        Vector3 destinationPoint = transform.position - 2 * (hit.collider.transform.position - transform.position);
                        if (destinationPoint.x > 26.5f)
                            destinationPoint.x = 26.5f;
                        else if(destinationPoint.x < -26.5f)
                            destinationPoint.x = -26.5f;

                        if(destinationPoint.y > 15)
                            destinationPoint.y = 15;
                        else if(destinationPoint.y < -15)
                            destinationPoint.y = -15;

                        targetPosition.Value = destinationPoint;
                        controller.SetNewDestination();
                    }
                }
                else if (hit.collider.tag == "Cheese" && run == false)
                {
                    hasTarget = true;
                    targetSeen = true;  
                    if (IsServer)
                    {
                        float cheeseDistance = Vector3.Distance(hit.collider.transform.position, transform.position);
                        if (cheeseDistance < minCheeseDistance)
                        {
                            minCheeseDistance = cheeseDistance;
                            targetPosition.Value = hit.collider.transform.position;
                        }
                    }
                }
                
            }

            if (hitTarget.collider != null) //Zjadanie sera
            {
                if (hitTarget.collider.tag == "Cheese")
                {
                    if (IsServer)
                    {
                        hitTarget.collider.GetComponent<RespawnCheese>().DestroyAndSpawn();

                        
                    }
                    if (updateSize == true)
                    {
                        updateSize = false;
                        StartCoroutine(MakeBigger());
                    }

                }
            }
        }
        if (targetSeen == false)
        {
            hasTarget = false;
        }


    }

    IEnumerator TurnOnRunForFewSeconds()
    {
        run = true;
        yield return new WaitForSeconds(3);
        run = false;
    }

    IEnumerator MakeBigger()
    {
        yield return new WaitForSeconds(0.1f);
        transform.localScale = new Vector3(transform.localScale.x + 0.05f, transform.localScale.y + 0.05f, transform.localScale.z);
        updateSize = true;
    }

}
