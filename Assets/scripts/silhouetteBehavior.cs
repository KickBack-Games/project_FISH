using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class silhouetteBehavior : MonoBehaviour
{
	public bool goingLeft;
	private SpriteRenderer spr;

	public Vector2 pos;
	public float speed;
	private float startX;
	private float startY;

	public float begin;
	public float end;

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
		yield return new WaitForSeconds(Random.Range(10, 15));
		StartCoroutine(swim());
	}

	public IEnumerator swim()
    {
        yield return new WaitForSeconds(Random.Range(begin, end));
        if (goingLeft)
		{
			if (transform.position.x <= -15)
			{
				spr.flipX = true;
				goingLeft = false;
				pos = new Vector2(startX, startY);
			}
			StartCoroutine(swim());
		}
		else
		{
			if (transform.position.x >= 15)
			{
				spr.flipX = false;
				goingLeft = true;	
				pos = new Vector2(startX - 31, startY);
			}
			StartCoroutine(swim());
			
		}
    }
}
