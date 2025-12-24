using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PurrNet;
public class playerHealth : NetworkBehaviour
{
    public SyncVar<int> health = new SyncVar<int>(100, ownerAuth:true);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            health.value++;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            health.value--;
        }
    }
}
