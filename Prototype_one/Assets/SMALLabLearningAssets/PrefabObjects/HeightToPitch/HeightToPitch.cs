using UnityEngine;
using System.Collections;

public class HeightToPitch : MonoBehaviour {
	
	public float startingPitch = 1.0f;
	public int timeToDecrease = 5;
	private bool playAudio = false;
	private float previousY = 0.0f;
	public float pitchMod = 0.3f;
	
	
	// Use this for initialization
	void Start () {
		startingPitch = GetComponent<AudioSource>().pitch;

		playAudio = true;	
		GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!pause && playAudio){	
			GetComponent<AudioSource>().pitch = transform.position.y * pitchMod;
		}
		
		
	}
	
	
	public bool pause=false;

	void OnPause(){
		pause = true;

		GetComponent<AudioSource>().Stop();
	}
	
	void OnPlay(){
		pause = false;
		if (playAudio){
			GetComponent<AudioSource>().Play();
		}
	}
	
	void OnStop(){
		GetComponent<AudioSource>().Stop();
		playAudio = false;
		GetComponent<AudioSource>().pitch = startingPitch;
		previousY = transform.position.y;
	}
	/*
	void OnStart(){
		Debug.Log("playing Audio");
		playAudio = true;	
		audio.Play();
	}
	*/

}
