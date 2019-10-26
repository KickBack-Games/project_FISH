using System;
using UnityEngine;
using UnityEngine.Audio;

public class SM : MonoBehaviour 
{
	public Sound[] sounds;
	public static SM instance;

	// Use this for initialization
	void Awake () 
	{
		if (instance == null)
			instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
		}
	}
	
	void Start() 
	{
		if (PlayerPrefs.GetInt("MuteMusic", 1) == 1)
			Play("Theme");

	}

	public void Stop(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.Stop();
	}
	// Update is called once per frame
	public void Play (string name) 
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.Play();
	}
}
