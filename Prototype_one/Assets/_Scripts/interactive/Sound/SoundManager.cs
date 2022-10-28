using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] sounds;
    private void Awake()
    {
/*        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
*/
        AddAudioSource();
    }

    private void AddAudioSource()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
        }
    }
    public void PlaySound(string name, bool shouldLoop)
    {
        Sound target = System.Array.Find(sounds, sound => sound.name == name);
        target.source.loop = shouldLoop;
        target.source.Play();
    }
}
