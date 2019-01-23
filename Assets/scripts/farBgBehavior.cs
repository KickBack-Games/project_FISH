using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class farBgBehavior : MonoBehaviour 
{
	public bool goingLeft;
	private SpriteRenderer spr;

	public Vector2 pos;
	public float speed;
	private float startX;
	private float startY;

	public float begin;
	public float end;

	public float depth1;
	public float depth2;

	// Use this for initialization
	void Start () 
	{
		spr = GetComponent<SpriteRenderer>();
		startX = transform.position.x;
		startY = transform.position.y;
		pos = new Vector2(startX, startY);
		StartCoroutine(onBegin());
	}
	
	// Update is called once per frame
	void Update () 
	{

		transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);
	}

	public IEnumerator onBegin()
	{
		yield return new WaitForSeconds(Random.Range(1, 12));
		StartCoroutine(swim());
	}

	public IEnumerator swim()
    {
        yield return new WaitForSeconds(Random.Range(begin, end));
        if (goingLeft)
		{
			if (transform.position.x <= -4)
			{
				spr.flipX = true;
				goingLeft = false;

				var y = Random.Range(depth1, depth2);
				transform.position = new Vector2(transform.position.x, y);

				pos = new Vector2(startX, y);
			}
			StartCoroutine(swim());
		}
		else
		{
			if (transform.position.x >= 4)
			{
				spr.flipX = false;
				goingLeft = true;	

				var y = Random.Range(depth1, depth2);
				transform.position = new Vector2(transform.position.x, y);

				pos = new Vector2(startX - 10, y);
			}

			StartCoroutine(swim());
			
		}
    }
}
