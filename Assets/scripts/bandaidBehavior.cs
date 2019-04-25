using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bandaidBehavior : MonoBehaviour 
{
	private float ySpd;
	private Vector2 pos;
	void Start () 
	{
		ySpd = 1f;
		pos = new Vector2(Random.Range(-3.0f,3.0f), -5.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{

		transform.position = Vector2.MoveTowards(transform.position, pos, Time.deltaTime * ySpd); 
		if (transform.position.y < -4.5f)
			Destroy(gameObject);	
	}
}
