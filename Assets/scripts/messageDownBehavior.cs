using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class messageDownBehavior : MonoBehaviour 
{
	public GameObject gm;
	private play pl;

	private float ySpd, xScale;
	// Use this for initialization
	void Start () 
	{
		pl = gm.GetComponent<play>();
		ySpd = .1f;
		xScale = .1f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (pl.msgDown)
		{
			if (pl.isGhost)
			{
				transform.localScale = new Vector2(-1.5f, 1.5f);
			}
			else
			{
				if (transform.localScale.x >= 0)
				{
					pl.messageSpr.sprite = pl.messageSprites[0];
				}
				if (transform.localScale.x < 1.5)
				{
					transform.localScale = new Vector2(transform.localScale.x + xScale, transform.localScale.y);	
				}
				
			}
			if (transform.position.y > -6.45f)
			{
				transform.position = new Vector2(transform.position.x, transform.position.y + ySpd);
				ySpd -= .01f;
			}
			else
			{
				ySpd = .1f;
				pl.setSettingsTextInvisible();
			}
		}
		else
		{
			if (transform.position.y < 0f)
			{
				transform.position = new Vector2(transform.position.x, transform.position.y + ySpd);
				ySpd += .01f;
			}
			else
			{
				ySpd = .1f;
				gameObject.SetActive(false);
				pl.setSettingsText();
			}
		}
	}
}
