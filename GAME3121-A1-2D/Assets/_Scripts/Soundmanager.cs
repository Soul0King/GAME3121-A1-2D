using System.Collections.Generic;
using UnityEngine;

public class Soundmanager : MonoBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();

    private AudioSource source;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayWallSFX()
    {
        source.PlayOneShot(clips[0]);
    }
    public void PlayObstacleSFX()
    {
        source.PlayOneShot(clips[1]);
    }
    public void PlayLoseSFX()
    {
        source.PlayOneShot(clips[2]);
    }
}
