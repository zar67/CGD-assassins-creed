using UnityEngine;

public class HayBale : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			collision.gameObject.GetComponent<PlayerMovement>().SetInsideHayBale(true);
			collision.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.grey;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			collision.gameObject.GetComponent<PlayerMovement>().SetInsideHayBale(false);
			collision.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
		}
	}
}
