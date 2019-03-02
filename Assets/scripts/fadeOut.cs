using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fadeOut : MonoBehaviour {
	private SpriteRenderer sr;
	// Use this for initialization
	public float i = 0;
	void Start () 
	{
		sr = GetComponent<SpriteRenderer>();
		sr.color = new Color(0f,0f,0f,i);
	}
	void Update()
	{
		if (i < 1f)
			i += .1f;
		else
			SceneManager.LoadScene("main");
		sr.color = new Color(0f,0f,0f,i);
	}
}
