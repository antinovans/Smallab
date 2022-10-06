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

using UnityEngine;
using System.Collections;


/*         For Reference   
		InnerDisc Pscore = innerDisc1.GetComponent<InnerDisc>();
            Pscore.PlayerScore();
            GameObject InnerDisc2 = GameObject.Find("InnerDiscBorder");
            InnerDiscBorder Pscore2 = InnerDisc2.GetComponent<InnerDiscBorder>();
            Pscore2.PlayerScore();
            //Play Tick Sound Every Round
            GameObject ST = GameObject.Find("SoundTick");
            ST.GetComponent<CallSounds>().PlaySound();
			
	*/




public class SoundSpeedScript : MonoBehaviour 
{

	float currentDist;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	public void Update () 
	{

	}
	
	public void setPlayRate(float rate)
	{
	
		if(rate==0)
		{
			rate=0.00000000000001F;
		}
		GetComponent<AudioSource>().pitch=(1/rate)*2;

		
	}
	
	public void PlaySound()
	{
		GetComponent<AudioSource>().Play();
	}
	public void StopSound()
	{
		GetComponent<AudioSource>().Stop();
	}
	
	public void OnPause()
	{
		GetComponent<AudioSource>().Pause();
	}
	public void OnPlay(){
		GetComponent<AudioSource>().Play();		
	}
	public void OnReset()
	{
		//audio.Stop();
	}
}
