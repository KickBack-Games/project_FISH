using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookBehavior : MonoBehaviour 
{
	// GAME MANAGER STUFF
	public GameObject gm;
	private play pScript;
	private Vector2 pos;
	public bool anchored;
	private float yStart;
	private float xStart;

	public bool lost;
	public GameObject fish;
	public bool hook;
	public playerMovement fishy;

	private bool repelled, started,move;

	// difficulty
	public float speed;
	private float wait1;
	private float wait2;
	public float difficulty;

	void Start () 
	{
    	pScript = gm.GetComponent<play>();
    	difficulty = 0;
		pos = transform.position;
		anchored = false;
		yStart = transform.position.y;
		xStart = transform.position.x;
		fishy = fish.GetComponent<playerMovement>();
		started = false;
		wait1 = 0f;
		wait2 = 2f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (pScript.inMenu)
		{
			

		}
		else
		{
			if (!started)
				{
					started = true;
					StartCoroutine(onBegin());
				}

			lost = fishy.lost;
			repelled = fishy.saved;

			if (hook)
			{
				if ((Mathf.Abs(fish.transform.position.x - transform.position.x)) < 1)
					pos = new Vector2(fish.transform.position.x, fish.transform.position.y);
			}
			else if (hook && lost)
			{
				if ((Mathf.Abs(fish.transform.position.x - transform.position.x)) < 1)
					pos = new Vector2(fish.transform.position.x, fish.transform.position.y);
				pos = new Vector2(fish.transform.position.x, yStart);
				speed = fishy.speed;
			}	
			if (repelled)
			{
				transform.position = new Vector2(xStart, yStart);
				anchored = false;
				pos = new Vector2(xStart, yStart);
			}

			if (!lost)
			{
				if (!anchored)
				{
					if (difficulty < 3.1f)
						difficulty = pScript.timer * .05f;
					speed = 6 + difficulty;
				}
				else
				{
					speed = 6;
				}
				//move = true;
				transform.position = Vector2.MoveTowards(transform.position, pos, Time.deltaTime * (speed));
			}
			else
			{
				if (!pScript.trophyBG)
				{    			
					if ((Mathf.Abs(fish.transform.position.x - transform.position.x)) < 1f)
					{
						transform.position = new Vector2(transform.position.x, fish.transform.position.y);
					}
					else
					{
						pos = new Vector2(transform.position.x, yStart);
						transform.position = Vector2.MoveTowards(transform.position, pos, Time.deltaTime * (speed));
					}
				}
				else
				{
					transform.position = new Vector2(xStart, yStart);
				}

			}
			hook = fishy.hooked;
		}
	}

	IEnumerator moveHook()
    {
        yield return new WaitForSeconds(Random.Range(wait1, wait2));

        // logic
        if (!anchored)
        {

        	// going down
        	anchored = !anchored;
        	if (!hook)
        	{
        	    wait1 = 1.5f;
        		wait2 = 5.25f;
        		pos = new Vector2(xStart, Random.Range(-3.8f, -2f));
        	}
        }
        else
        {
        	anchored = !anchored;
        	if (!hook)
        	{
        		wait1 = 1.6f;
        		wait2 = 4f;
        		pos = new Vector2(xStart, 6f);
        	}
        }
        StartCoroutine(moveHook());
    }
    IEnumerator onBegin()
    {
    	yield return new WaitForSeconds(0);
    	StartCoroutine(moveHook());
    }
}
