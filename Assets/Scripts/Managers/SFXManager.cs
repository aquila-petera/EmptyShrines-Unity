using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private static SFXManager instance;
    private static AudioClip currentFootstepSound;

    [SerializeField]
    private AudioClip footstepSound;

    private void Awake()
    {
        currentFootstepSound = footstepSound;
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public static void PlaySound(AudioSource source, AudioClip clip, bool interrupt = false, float volume = 1f, float pitch = 1f)
    {
        if ((!interrupt && source.isPlaying) || (source.isPlaying && source.clip == clip))
            return;

        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(clip);
    }

    public static AudioClip GetFootstepSound()
    {
        return currentFootstepSound;
    }
}
