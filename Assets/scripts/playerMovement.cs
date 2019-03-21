using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour 
{

	// GAME MANAGER STUFF
	public GameObject gm;
	private play pScript;

	// OTHER STUFF
	private SpriteRenderer sprite;
	public bool hooked;
	public bool lost;

	private Vector3 pos;
	public float speed = 9.0f;
	//private Transform tr;

	public float counter;
	public int safetyGoal;
	public bool saved;
	private hookBehavior hook;

	public float z;
	private Quaternion newQuaternion;

	public float rot;
	public float rotHooked;
	public float tim;
	public float timHooked;
	public float life;
	public float damage;
	public bool lookingRight;

	public GameObject bubble;
	private bool move;

	// Use this for initialization
	void Start () 
	{
    	pScript = gm.GetComponent<play>();

		pos = transform.position;
		//tr = transform;

		sprite = GetComponent<SpriteRenderer>();

		hooked = false;
		lost = false;
		saved = false;
		rot = 0;
		tim = .005f;
		timHooked = .05f;
		// rotation
		newQuaternion = new Quaternion();
        newQuaternion.Set(0, 0, z, 0);
        lookingRight = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		var mousePos = Input.mousePosition;

		if (pScript.inMenu)
		{
			
			moveRotation();
			move = false;
		}
		else
		{
			if (lost)
			{
				pos = new Vector2(transform.position.x, 5.8f);
				hookRotation();
				life = 0;
				move = false;
			}
			else
			{
				if (hooked)
				{
					life -= damage * Time.deltaTime;
					rot = 0;
			    	hookRotation();

					if (life <= 0)
					{
						life = 0;
						lost = true;
					}
					else
					{
						if (Input.GetMouseButtonDown(0))
						{
							counter++;

						}	
						if (counter >= safetyGoal)
						{
							hooked = false;
							saved = true;
						}
					}
				}
				else
				{
					moveRotation();
					rotHooked = .5f;
					// reset
					counter = 0;
					saved = false;
					if (Input.GetMouseButtonDown(0) && !hooked)
					{
						if ((mousePos.x >= Screen.width *.5f) && (pos.x < 2.5f))
						{
		                	TapRight();
		                }
						else if ((mousePos.x < Screen.width *.5f) && (pos.x > -2.5f)) 
						{
							TapLeft();
						}
					}
				}
			}
			if (lookingRight)
			{
           		if (transform.localScale.x < 1f)
           		{
        			transform.localScale += new Vector3(.25f, 0, 0);
        		}
			}
			else
			{
           		if (transform.localScale.x > -1f)
           		{
        			transform.localScale += new Vector3(-.25f, 0, 0);
        		}
			}
			transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);	
		}
		transform.rotation = newQuaternion;	
	}

	/*void FixedUpdate()
	{
		if (move)
			transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);	
		transform.rotation = newQuaternion;	
	}*/
    public void TapLeft()
    {
    	lookingRight = false;
        pos += Vector3.left * 1.5f;
        if (pos.x < -2.26f)
        	pos = new Vector2(-2.25f, pos.y);
    }
    public void TapRight()
    {   
    	lookingRight = true;
        pos += Vector3.right * 1.5f;
        if (pos.x > 2.26f)
        	pos = new Vector2(2.25f, pos.y);
    }

    void hookRotation()
    {
		if (rotHooked <= .4f)
			timHooked *= -1;
		if (rotHooked >= .6f)
			timHooked *= -1;

		rotHooked += timHooked;
		if (transform.localScale.x == 1f)
			newQuaternion.Set(0, 0, rotHooked, .5f);
		else
			newQuaternion.Set(0, 0, -rotHooked, .5f);	
    }
    void moveRotation()
    {
		if (rot <= -.05f)
			tim *= -1;
		if (rot >= .05f)
			tim *= -1;

		rot += tim;
		if (sprite.flipX)
			newQuaternion.Set(0, 0, rot, .5f);
		else
			newQuaternion.Set(0, 0, -rot, .5f);	
    }

    void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "hook") // For the opponents hitbox
		{
			if (!hooked)
			{
				if ((transform.position.y > other.transform.position.y))
				{
					life -= 30;
				}
				else 
				{
					life -= 20;
				}
				for(int i = 0; i < 3; i++)
					Instantiate(bubble, transform.position, newQuaternion);
			}
			
			hooked = true;

			pos = new Vector2(other.transform.position.x, transform.position.y);
		}
	}
}
