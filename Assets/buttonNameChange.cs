using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class buttonNameChange : MonoBehaviour
{
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void changeName(GameObject textBox)
    {
        string playername =  textBox.GetComponent<TMPro.TextMeshProUGUI>().text;
        textBox.SetActive(false);
        gameObject.SetActive(false);
    }
}
