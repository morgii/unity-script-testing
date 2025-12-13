using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballInit : MonoBehaviour
{

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * 500);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
