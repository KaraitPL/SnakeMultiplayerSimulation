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

    public bool hasTarget = false;

    private void Awake()
    {
        targetPosition.Value = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        bool targetSeen = false;
        for (int angle = -90; angle <= 90; angle++)
        {
            Vector3 ray = Quaternion.Euler(0, 0, angle) * transform.right;
            LayerMask mask = LayerMask.GetMask("SnakeLayer");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, 4f, mask);
            Debug.DrawRay(transform.position, ray * 4, new Color(0, 0, 255, 0.10f));  //Rysuje radar
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Player")
                {
                    hasTarget = true;
                    targetSeen = true;
                    if (IsServer)
                    {
                        targetPosition.Value = -4 * (hit.collider.transform.position - transform.position);
                    }
                }
            }
        }
        if (targetSeen == false)
        {
            hasTarget = false;
        }


    }
}
