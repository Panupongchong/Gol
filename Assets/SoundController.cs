using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	public Sound[] sounds;
	private Dictionary<string, AudioSource> source = new Dictionary<string, AudioSource> ();

	//Singleton
	private static SoundController instance = null;
	public static SoundController Instance
	{
		get
		{ 
			return instance; 
		}
	}

	void Awake(){
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (this);
			return;
		}
		
		foreach (Sound sound in sounds) {
			AudioSource s = gameObject.AddComponent<AudioSource>();
			s.clip = sound.clip;
			s.volume = sound.volume;
			s.loop = sound.loop;
			source [sound.name] = s;
		}
		PlaySound ("BGM");
	}

	public void PlaySound(string name){
		if (source.ContainsKey (name))
			source [name].Play ();
	}

	// Update is called once per frame
	[System.Serializable]
	public class Sound {
		public string name;
		public AudioClip clip;
		[Range(0f,1f)]
		public float volume;
		public bool loop;
	}
}
