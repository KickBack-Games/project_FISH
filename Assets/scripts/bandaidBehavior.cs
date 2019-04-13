using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bandaidBehavior : MonoBehaviour 
{
	private GameObject gm;
	private play pl;
	public SpriteRenderer spr;

	private float ySpd, xScale;
	private Vector2 pos;
	// Use this for initialization
	public bool switch_, faceup;
	void Start () 
	{
		spr = GetComponent<SpriteRenderer>();
		gm = GameObject.FindWithTag("gm");
		pl = gm.GetComponent<play>();
		switch_ = true;
		faceup = false;
		ySpd = 1f;
		pl = gm.GetComponent<play>();
		pos = new Vector2(Random.Range(-3.0f,3.0f), -5.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (switch_)
		{
			transform.localScale += new Vector3(-.05f, 0, 0);
	       	if (transform.localScale.x <= -1f)
	       	{
	    		switch_ = false;
			}	
			if (transform.localScale.x <= 0)
				faceup = false;

		}
		else
		{           	
			transform.localScale += new Vector3(.05f, 0, 0);	
    		if (transform.localScale.x >= 1)
    		{
    			switch_ = true;
    		}
   			if (transform.localScale.x >= 0)
				faceup = true;
		}

		print(faceup);
		if(faceup)
		{
			pl.bandaidSpr.sprite = pl.bandaidSprites[0];
			spr.sprite = pl.bandaidSprites[0];
		}
		else
		{
			print("NOW");
			pl.bandaidSpr.sprite = pl.bandaidSprites[1];
			spr.sprite = pl.bandaidSprites[1];
		}
		transform.position = Vector2.MoveTowards(transform.position, pos, Time.deltaTime * ySpd); 
		if (transform.position.y < -4.5f)
			Destroy(gameObject);	
	}
}
