using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] sounds;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

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
    public void PlaySoundOneShotMultipleTimes(string name, int times)
    {
        Sound target = System.Array.Find(sounds, sound => sound.name == name);
        /*target.source.PlayOneShot(target.clip);*/
        StartCoroutine(PlayOneShot(target, times));
    }
    IEnumerator PlayOneShot(Sound s, int times)
    {
        while (times != 0)
        {
            s.source.PlayOneShot(s.clip);
            yield return new WaitForSeconds(0.1f);
            times--;
        }
    }
}
