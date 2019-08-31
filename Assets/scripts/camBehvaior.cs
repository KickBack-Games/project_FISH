using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camBehvaior : MonoBehaviour 
{
	public GameObject gm;
	private play pScript;

	public playerMovement fishy;
	public GameObject fish;
	public float view;
	private Vector2 pos;

	public Camera m_OrthographicCamera;
    //These are the positions and dimensions of the Camera view in the Game view
    public float m_ViewPositionX, m_ViewPositionY, m_ViewWidth, m_ViewHeight;

    void Start()
    {
    	pScript = gm.GetComponent<play>();
    	pos = new Vector3(fish.transform.position.x, 0, -10f);
    	m_ViewPositionX = 0;
    	m_ViewPositionY = 0;
    	m_ViewWidth = 1;
    	m_ViewHeight = 1;
		m_OrthographicCamera.enabled = true;
		fishy = fish.GetComponent<playerMovement>();
		m_OrthographicCamera.rect = new Rect(m_ViewPositionX, m_ViewPositionY, m_ViewWidth, m_ViewHeight);	
	}
	
	// Update is called once per frame
	void Update () 
	{
        m_OrthographicCamera.orthographic = true;
        //Set the size of the viewing volume you'd like the orthographic Camera to pick up (5)
        m_OrthographicCamera.orthographicSize = view;
        if (pScript.inMenu)
        {
        	if (pScript.inSettings)
        	{
        		pos = new Vector3(fish.transform.position.x, fish.transform.position.y - 10.75f, -10);
        		view = 5.5f;
        	}
        	else if (pScript.inTutorial)
        	{
        		pos = new Vector3(fish.transform.position.x, fish.transform.position.y - 21.75f, -10);
        		view = 5.5f;

        	}
        	else
        	{
        		// center on fish
        		pos = new Vector3(fish.transform.position.x, fish.transform.position.y, -10);
        		view = 5.5f;
        	}
        }
        else
        {
			if (fishy.hooked && !fishy.lost)
			{
				pos = new Vector3(fish.transform.position.x, fish.transform.position.y, -10);
				if (view > 3f)
					view -= 10f * Time.deltaTime;
			}
			else
			{
				pos = new Vector3(0, 0, -10);
				if (view < 5f)
					view += 10f * Time.deltaTime;
			}        	
        }

		transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 30f);	
		transform.position = new Vector3 (transform.position.x, transform.position.y, -10);
	}
}
