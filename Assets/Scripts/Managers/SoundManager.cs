using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private AudioSource _audioSource;
    private float _volume = 1f;
    
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; }
    }

    private void Start()
    {
        _audioSource = new AudioSource();
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * _volume);
    }
    
    // TODO: Keep this method for when wanted to play random sounds from an array.
    // private void  PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
    //     PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    // }

    public void PlayExplosionSound(Vector3 position)
    {
        PlaySound(audioClipRefsSO.explosion, position);
    }

    public void PlayHitSound(Vector3 position)
    {
        PlaySound(audioClipRefsSO.hit, position);
    }
}