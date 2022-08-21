using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShiki : BasicEnemy
{
    [SerializeField]
    private List<string> promptList;
    [SerializeField]
    private List<Transform> travelPoints;
    [SerializeField]
    private List<GameObject> triggeredObjects;
    [SerializeField]
    private float duration;

    private TextLabel prompt;
    private int promptIdx;
    private List<Vector3> waypoints;
    private float timer;
    private List<SpriteRenderer> sprites;
    private SpriteFlipper spriteFlipper;

    public override void Die()
    {
        if (promptIdx < promptList.Count - 1)
        {
            promptIdx++;
            prompt.SetBaseString(promptList[promptIdx]);
            transform.position = waypoints[promptIdx];
            spriteFlipper.SetFacing(false);
            prompt.SetEnabled(false);
            ParticleManager.PlayParticleSystemAtPosition("SmokePuff", transform.position, new Color(0.8f, 0.2f, 0.8f));
            timer = duration;
        }
        else
        {
            StopAllCoroutines();
            foreach (GameObject obj in triggeredObjects)
            {
                obj.GetComponent<IActivatable>().Activate();
            }
            base.Die();
        }
    }

    new protected void Start()
    {
        base.Start();
        waypoints = new List<Vector3>();
        waypoints.Add(transform.position);
        prompt = textPrompts[0];
        prompt.SetBaseString(promptList[0]);
        sprites = new List<SpriteRenderer>();
        sprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
        spriteFlipper = GetComponent<SpriteFlipper>();
        foreach (Transform t in travelPoints)
        {
            waypoints.Add(t.position);
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer > 3)
            {
                foreach (var sprite in sprites)
                    sprite.color = new Color(1f, (int)(timer * 10) % 5 == 0 ? 0.2f : 1f, 1f);
            }
            else
            {
                foreach (var sprite in sprites)
                    sprite.color = new Color(1f, (int)(timer * 10) % 2 == 0 ? 0.2f : 1f, 1f);
            }

            if (timer <= 0)
            {
                foreach (var sprite in sprites)
                    sprite.color = Color.white;
                promptIdx = 0;
                prompt.SetBaseString(promptList[promptIdx]);
                transform.position = waypoints[promptIdx];
                spriteFlipper.SetFacing(false);
                prompt.SetEnabled(false);
                ParticleManager.PlayParticleSystemAtPosition("SmokePuff", transform.position, new Color(0.8f, 0.2f, 0.8f));
            }
        }
    }
}
