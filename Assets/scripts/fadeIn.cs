using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadeIn : MonoBehaviour {
	private SpriteRenderer sr;
	// Use this for initialization
	public float i = 1;

    void Start () 
	{
		sr = GetComponent<SpriteRenderer>();
		sr.color = new Color(0f,0f,0f,i);
	}
	void Update()
	{
		if (i > 0f)
			i -= .02f;
		else
			Destroy(this.gameObject);
		sr.color = new Color(0f,0f,0f,i);
	}
}
