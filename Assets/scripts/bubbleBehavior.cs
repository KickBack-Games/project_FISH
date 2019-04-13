using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubbleBehavior : MonoBehaviour 
{

	private SpriteRenderer sprite;

	private float speedX;
	private float speedY;
	private float count;
 	private float size;

	public float edge;
	public string type;


     void Start()
     {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = Random.Range(-1, 2);
		size = Random.Range(.5f, .9f);
		transform.localScale = new Vector2(size, size);

		if (type == "bubble")
		{
			speedY = 7f;
			speedX = Random.Range(-.8f, .8f);
			count = .1f;
		}
		else
		{
			speedY = Random.Range(4.0f, 7.0f);
			speedX = Random.Range(-.8f, .8f);
			count = .3f;

		}
	}
	
	// Update is called once per frame
	void Update () 
	{

		speedX += count;
		if (speedX > edge)
		{
			count *= -1;
		}
		else if (speedX < -edge)
		{
			count *= -1;
		}
		

		transform.position = new Vector2(transform.position.x + speedX * Time.deltaTime, transform.position.y + speedY * Time.deltaTime);
	}
}
