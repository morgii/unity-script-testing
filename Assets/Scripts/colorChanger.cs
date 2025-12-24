using HutongGames.PlayMaker.Actions;
using PurrNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorChanger : NetworkBehaviour
{

    private Material boxMaterial;

    void Start()
    {
        boxMaterial = GetComponent<Renderer>().sharedMaterial;
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeColor(Color.red);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeColor(Color.green);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeColor(Color.blue);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeColor(Color.black);
        }
    }


    [ServerRpc(requireOwnership: false)]
    void ChangeColor(Color color)
    {
            ChangeColorForClients(color);
    }

    [ObserversRpc]
    void ChangeColorForClients(Color color)
    {
        boxMaterial.color = color;
    }
}
