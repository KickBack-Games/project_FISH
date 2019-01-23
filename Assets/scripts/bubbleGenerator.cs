using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubbleGenerator : MonoBehaviour {

	private float x;
	private float y;
	public GameObject bubble;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(OnBegin());	
		y = transform.position.y;
	}

	public IEnumerator OnBegin()
    {
        yield return new WaitForSeconds(Random.Range(3, 8));
        x = Random.Range(-2.5f, 2.5f);
        Instantiate(bubble, new Vector2(x,y), Quaternion.identity);
        StartCoroutine(OnBegin());
    }
}
