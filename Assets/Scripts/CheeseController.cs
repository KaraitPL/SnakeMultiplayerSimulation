using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseController : MonoBehaviour
{

    CheeseView cheeseView;
    Vector3 spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        cheeseView = GetComponent<CheeseView>();
        transform.position = new Vector3(Random.RandomRange(-5, 5), Random.RandomRange(-5, 5), 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
