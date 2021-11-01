using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiveScript : MonoBehaviour
{

    public float delay = 5f;
    private int ammo = 0;


    void OnTriggerEnter2D()
    {
        StartCoroutine("Delaythis");
        Debug.Log("Destroyed Dive Box");
    
    }

    IEnumerator Delaythis()
    {

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);


    }


}