using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accessoryBehavior : MonoBehaviour 
{
	public GameObject fish;
	private SpriteRenderer fishSpr;
	private SpriteRenderer spr;
	// Use this for initialization
	void Start () 
	{
		fishSpr = fish.GetComponent<SpriteRenderer>();
		spr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (fishSpr.flipX)
			spr.flipX = true;
		else
			spr.flipX = false;
	}
}
