/*

Copyright © 2011.  Arizona Board of Regents.  All Rights Reserved
 
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
POSSIBILITY OF SUCH DAMAGE. 

*/

// Module:	CircularTimer2.cs
// Author: 	Rob Galante
// Date:	June 15, 2011
// Version:	2.0

// ****************************************************************************
// Modification Log:
//	06-15-11 RMG:	This version of the CircularTimer class, CircularTimer2, has no OpenGL calls. It uses
//						a sprite sheet which contains 16 images, although it will support other dimensions. 
//						These images reflect the state of the circular timer as it rotates over time. See the
//						script, AnimatedTexture, for details on how the sprite sheet is rendered.
//  07-19-11 RMG:	Play the sound when the game starts.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimatedTexture))]
public class CircularTimer : MonoBehaviour {

	#region Properties
	// Public Properties	
	public float clockCycleDuration = 1.0f;
	public int clockCycles = 1;
	public float elapsedTime = 0.0f;
	public bool enableClockCycleSound;
	public AudioClip CycleSound;
	public GameObject CycleSoundPrefab;
	public bool isRunning;
	
	// 05-25-11 RMG:	New events
	public delegate void CycleCompleteEventHandler(float elapsedTime);
	public event CycleCompleteEventHandler CycleComplete;
	public delegate void AllCyclesCompleteEventHandler(float elapsedTime);
	public event AllCyclesCompleteEventHandler AllCyclesComplete;
	public delegate void ResetEventHandler();
	public event ResetEventHandler Reset;

	// Private Properties	
	private AnimatedTexture animatedTexture;
	private int completedClockCycles = 0;
	private GameObject cycleSoundGameObject;
	private AudioSource cycleSoundAudioSource;
	private bool isContinuous = false;
	private Vector3 offset;
	#endregion
	
	#region Event Handlers
	// Update is called once per frame
	void FixedUpdate () 
	{
		// Don't do anything if the clock isn't running
		if(isRunning)
		{
			if(completedClockCycles < clockCycles || isContinuous)
			{
				if(elapsedTime < clockCycleDuration)
				{
					elapsedTime += Time.deltaTime;
				}
				else
				{
					// 05-24-11 RMG: When clock cycle completes, play sound and notify listeners
					StartCoroutine(OnCycleComplete(elapsedTime));
					
					elapsedTime = 0.0f;
					completedClockCycles++;
				}
			}
			else
			{
				
				OnAllCyclesComplete(); // notify listeners
				OnReset();
			}
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		if(clockCycles == -1)
		{
			isContinuous = true;
		}
		// Get a reference to our AnimatedTexture
		animatedTexture = GetComponent<AnimatedTexture>();
		
		// Instantiate the CycleSoundPrefab, if one exists
		if (CycleSoundPrefab != null && CycleSound != null)
		{
			cycleSoundGameObject = Instantiate(CycleSoundPrefab) as GameObject;
			if (cycleSoundGameObject != null)
			{
				cycleSoundAudioSource = cycleSoundGameObject.GetComponent<AudioSource>();
				if (cycleSoundAudioSource != null)
					cycleSoundAudioSource.clip = CycleSound;
			}
		}
	}	
	#endregion

	#region Public Methods
	// Note: I know this word is spelled wrong, but I'm not sure who is calling it. So I have to leave it.
	public float GetEllapsedTime()
	{
		float totalTime = elapsedTime + (completedClockCycles * clockCycleDuration);
		return totalTime;
	}
	
	public void OnPause()
	{
		OnStop();
	}
	
	public void OnPlay()
	{
		isRunning = true;
		// Turn on the animated texture
		if (animatedTexture != null)
		{
			animatedTexture.IsEnabled = true;
		}
		// Play the sound when the game starts
		StartCoroutine(PlaySound());
	}
	
	public void OnReset()
	{
		OnStop();
		completedClockCycles = 0;
		elapsedTime = 0.0f;		
		if (Reset != null)
			Reset();
	}
	
	public void OnStop()
	{
		isRunning = false;
		// Turn off the animated texture
		if (animatedTexture != null)
		{
			animatedTexture.IsEnabled = false;
		}
	}	
	#endregion
	
	#region Private Methods	
	void OnAllCyclesComplete()
	{
		// Notify listeners
		if (AllCyclesComplete != null)
			AllCyclesComplete(elapsedTime);
		
		//print("Completed Requested Number Of Clock Cycles: " + numCompletedCycles);
	}
	
	private IEnumerator OnCycleComplete(float elapsedTime)
	{
		
		StartCoroutine(PlaySound());
		
		
		// Notify listeners
		if (CycleComplete != null)
			CycleComplete(elapsedTime);
		
		yield return 0;
	}
	
	private IEnumerator PlaySound()
	{
		if (enableClockCycleSound && cycleSoundAudioSource != null)
		{
			
			cycleSoundAudioSource.PlayOneShot(cycleSoundAudioSource.clip);
		}
		yield return 0;
	}	
	#endregion
}
