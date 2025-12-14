using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    private Transform playerTransform;
    private Material playerMaterial;
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerMaterial = GetComponent<Renderer>().material;
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        playerMaterial.SetColor("_Color", randomColor);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement2d = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 playerMovement = new Vector3(movement2d.x, 0, movement2d.y).normalized;
        playerTransform.Translate(playerMovement * Time.deltaTime * 5f, Space.World);
        Debug.Log("Update running");


    }
}
