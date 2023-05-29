using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RatRespawn : NetworkBehaviour
{
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider;

    [SerializeField] GameObject holeA;
    [SerializeField] GameObject holeB;
    [SerializeField] GameObject holeC;

    Vector3 deadPosition;
    public void MakeRespawnAction()
    {

            deadPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;

            collider = GetComponent<CapsuleCollider2D>();
            collider.enabled = false;

            transform.position = new Vector3(-10000, -10000, 0);

            StartCoroutine(SpawnInHole());
        
        

    }

    IEnumerator SpawnInHole()
    {
        yield return new WaitForSeconds(5);

        

        float dist1 = Vector3.Distance(deadPosition, holeA.transform.position);
        float dist2 = Vector3.Distance(deadPosition, holeB.transform.position);
        float dist3 = Vector3.Distance(deadPosition, holeC.transform.position);

        Vector3 maxDistance;

        if (dist1 > dist2 && dist1 > dist3)
            maxDistance = holeA.transform.position;
        else if (dist2 > dist1 && dist2 > dist3)
            maxDistance = holeB.transform.position;
        else
            maxDistance = holeC.transform.position;





        transform.position = maxDistance;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.enabled = true;
        collider.enabled = true;


    }
}
