using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenBehavior : MonoBehaviour 
{
	private float dimensions;
	private float acc;
	private float countdown;
	// Use this for initialization
	void Start () {
		dimensions = 0;
		acc = .00001f;
		countdown = 11;
	}
	
	// Update is called once per frame
	void Update () {
		if (dimensions < .85f) {
			dimensions += acc + Time.deltaTime * 1f;
			acc += .00005f;
		} else {
			if (countdown >= 0f){
				countdown -= .1f;
			} else {
				SceneManager.LoadScene("main");
			}
		}
		transform.localScale = new Vector2(dimensions, dimensions);
	}
}
