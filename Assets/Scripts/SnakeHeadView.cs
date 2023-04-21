using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SnakeHeadView : NetworkBehaviour
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
        //Vector3 leftRay = Quaternion.Euler(0, 0, -45) * transform.right;
        //Vector3 rightRay = Quaternion.Euler(0, 0, 45) * transform.right;
        for (int angle = -45; angle <= 45; angle++)
        {
            
            Vector3 ray = Quaternion.Euler(0, 0, angle) * transform.right;
            LayerMask mask = LayerMask.GetMask("TargetObjects");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, 4f, mask);
            Debug.DrawRay(transform.position, ray * 4, new Color(255,0,0,0.10f));  //Rysuje radar
            //Debug.DrawRay(transform.position, leftRay * 4, Color.red);
            //Debug.DrawRay(transform.position, rightRay * 4, Color.red);
            if (hit.collider != null)
            {
                
                if (hit.collider.tag == "Rat")
                {
                    
                    targetSeen = true;
                    hasTarget = true;
                    if (IsServer)
                    {
                        targetPosition.Value = hit.collider.transform.position;
                    }
                    
                }



            }
        }
        if(targetSeen == false)
        {
            hasTarget = false;
        }

        
    }
}