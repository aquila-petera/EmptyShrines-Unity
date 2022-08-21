using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public static void PlayParticleSystemAtPosition(string name, Vector3 position)
    {
        PlayParticleSystemAtPosition(name, position, Color.white);
    }

    public static void PlayParticleSystemAtPosition(string name, Vector3 position, Color color)
    {
        GameObject particlePrefab = Resources.Load<GameObject>($"Prefabs/Particles/ParticleEmitter{name}");
        if (particlePrefab == null)
        {
            Debug.LogError($"Error: Particle system '{name}' not found!");
            return;
        }
        GameObject particleObj = Instantiate(particlePrefab);
        ParticleSystem particleSystem = particleObj.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startColor = color;
        particleObj.transform.position = position;
        particleSystem.Play();
    }
}
