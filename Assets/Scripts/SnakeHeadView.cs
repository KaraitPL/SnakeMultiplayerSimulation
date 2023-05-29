using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SnakeHeadView : NetworkBehaviour
{
    public NetworkVariable<Vector3> targetPosition = new(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Server);
    //public GameObject playerSetupUI;
    //private PlayerSetup playerSetup;

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
        float minRatDistance = 100000f;
        for (int angle = -45; angle <= 45; angle++)
        {

            Vector3 ray = Quaternion.Euler(0, 0, angle) * transform.right;
            Vector3 rayTarget = transform.right;
            LayerMask mask = LayerMask.GetMask("RatLayer");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, 4f, mask);
            RaycastHit2D hitTarget = Physics2D.Raycast(transform.position, rayTarget, 0.5f, mask);
            Debug.DrawRay(transform.position, ray * 4, new Color(255, 0, 0, 0.10f));  //Rysuje radar
            Debug.DrawRay(transform.position, rayTarget * 0.5f, new Color(0, 255, 0, 1f));
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
                        float ratDistance = Vector3.Distance(hit.collider.transform.position, transform.position);
                        if (ratDistance < minRatDistance)
                        {
                            minRatDistance = ratDistance;
                            targetPosition.Value = hit.collider.transform.position;
                        }
                    }
                }
            }

            if (hitTarget.collider != null)
            {
                if (hitTarget.collider.tag == "Rat")
                {
                        RatRespawn ratRespawn = hitTarget.collider.GetComponent<RatRespawn>();
                        ratRespawn.MakeRespawnAction();
                    
                        
                    
                    //playerSetup.GetComponent<PlayerSetup>().SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 2);
                }
            }
        }
        if (targetSeen == false)
        {
            hasTarget = false;
        }


    }

}