using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightshade : MonoBehaviour 
{
	public GameObject s2;
	private SpriteRenderer sr1,sr2;
	private float f1, f2;
	public float count;
	private bool go;
	// Use this for initialization
	void Start () 
	{
		sr1 = GetComponent<SpriteRenderer>();
		sr2 = s2.GetComponent<SpriteRenderer>();
		sr1.color = new Color(1f,1f,1f,0f);
		sr2.color = new Color(1f,1f,1f,1f);
		f1 = 1f;
		f2 = 1f-f1;
		count = .01f;
		go = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(go==true)
		{
			f1 += count;

			if (f1 > 1)
			{
				f1 = 1f;
				count *= -1;
				go = false;
				StartCoroutine(onBegin());
			}
			else if (f1 < 0)
			{
				f1 = 0f;
				count *= -1;
				go = false;
				StartCoroutine(onBegin());
			}
		}
		f2 = 1f-f1;

		sr1.color = new Color(1f,1f,1f,f1);
		sr2.color = new Color(1f,1f,1f,f2);	
	}
	IEnumerator onBegin()
    {
    	yield return new WaitForSeconds(.7f);
    	go = true;
    }
}
