using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayBale : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			Debug.Log("Enter");
			collision.gameObject.GetComponent<PlayerMovement>().SetInsideHayBale(true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			Debug.Log("Exit");
			collision.gameObject.GetComponent<PlayerMovement>().SetInsideHayBale(false);
		}
	}
}
