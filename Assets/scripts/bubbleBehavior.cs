using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubbleBehavior : MonoBehaviour 
{

	private SpriteRenderer sprite;

	private float speedX;
	private float speedY;
	private float count;
 
     void Start()
     {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = Random.Range(-1, 2);
		var size = Random.Range(.5f, .9f);
		transform.localScale = new Vector2(size, size);

		speedY = 7f;
		speedX = Random.Range(-.8f, .8f);
		count = .1f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		speedX += count;
		if (speedX > 1)
		{
			count *= -1;
		}
		else if (speedX < -1)
		{
			count *= -1;
		}
		

		transform.position = new Vector2(transform.position.x + speedX * Time.deltaTime, transform.position.y + speedY * Time.deltaTime);
	}
}
