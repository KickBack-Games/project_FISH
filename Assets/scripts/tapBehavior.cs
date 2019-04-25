using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tapBehavior : MonoBehaviour 
{
	
	// Update is called once per frame
	void Update () 
	{
		transform.localScale = new Vector2(transform.localScale.x - .03f, transform.localScale.y - .03f);
		if (transform.localScale.x <= 0)
			Destroy(this.gameObject);
	}
}
