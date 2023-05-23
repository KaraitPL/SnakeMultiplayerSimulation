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

    private void Awake()
    {
        controller = GetComponent<RatController>();
        targetPosition.Value = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        bool targetSeen = false;
        for (int angle = -90; angle <= 90; angle++)
        {
            Vector3 ray = Quaternion.Euler(0, 0, angle) * transform.right;
            LayerMask mask = LayerMask.GetMask("SnakeLayer") | LayerMask.GetMask("CheeseLayer");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, 4f, mask);
            Debug.DrawRay(transform.position, ray * 4, new Color(0, 0, 255, 0.10f));  //Rysuje radar
            if (hit.collider != null)
            {
                Debug.Log("Hit");
                if (hit.collider.tag == "Player")
                {
                    hasTarget = true;
                    targetSeen = true;
                    Debug.Log("RUN");
                    if (IsServer)
                    {
                        targetPosition.Value = -2 * (hit.collider.transform.position - transform.position);
                        controller.SetNewDestination();
                    }
                }
                else if (hit.collider.tag == "Cheese")
                {
                    //cheeseSpotted = true;
                    //targetSeen = true;
                    Debug.Log("SERRR");
                    //targetPosition.Value = hit.collider.transform.position;
                    //controller.SetNewDestination();
                }
            }
        }
        if (targetSeen == false)
        {
            hasTarget = false;
        }


    }
}
