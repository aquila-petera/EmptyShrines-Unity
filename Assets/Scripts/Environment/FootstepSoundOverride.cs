using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundOverride : MonoBehaviour
{
    [SerializeField]
    private AudioClip footstepSound;

    public AudioClip GetFootstepSound()
    {
        return footstepSound;
    }
}
