using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarController : MonoBehaviour
{
    [SerializeField]
    private GameObject shidePrefab;

    private List<HealthShideManager> shides;
    private int shideIndex = 0;
    private Vector3 startPos;

    private void Start()
    {
        EventManager.BindEvent(typeof(PlayerHealthChangeEvent), OnPlayerHealthChanged);
        EventManager.BindEvent(typeof(PlayerSpawnedEvent), OnPlayerSpawn);
        startPos = transform.position;
    }

    private void OnDestroy()
    {
        EventManager.UnbindEvent(typeof(PlayerHealthChangeEvent), OnPlayerHealthChanged);
        EventManager.UnbindEvent(typeof(PlayerSpawnedEvent), OnPlayerSpawn);
    }

    private void OnPlayerHealthChanged(BasicEvent e)
    {
        int amt = (e as PlayerHealthChangeEvent).damage;
        if (amt > 0) // damage
        {
            for (int i = 0; i < amt; i++)
            {
                shides[shideIndex].Flip();
                shideIndex--;
            }
            StartCoroutine(DoShake());
        }
        else // heal
        {
            for (int i = 0; i < -amt; i++)
            {
                shides[shideIndex + 1].Flip();
                shideIndex++;
            }
        }
    }

    private void OnPlayerSpawn(BasicEvent e)
    {
        if (shides == null)
        {
            shides = new List<HealthShideManager>();
        }

        foreach (var shide in shides)
        {
            Destroy(shide.gameObject);
        }

        shides.Clear();
        int maxHp = EntityManager.GetPlayerMaxHp();
        int hp = EntityManager.GetPlayerHp();
        for (int i = 0; i < maxHp; i++)
        {
            var shide = Instantiate(shidePrefab, transform).GetComponent<HealthShideManager>();
            shides.Add(shide);
            if (i == 0)
            {
                shide.SetIsLeftmost(true);
            }
            if (i == maxHp - 1)
            {
                shide.SetIsRightmost(true);
            }
            if (i >= hp)
            {
                shide.FlipInstant();
            }
        }
        shideIndex = hp - 1;
    }

    private IEnumerator DoShake()
    {
        float param = 0;
        while (param < Mathf.PI * 6)
        {
            param += Time.deltaTime * Mathf.PI * 12;
            transform.position = startPos + new Vector3(Mathf.Sin(param) * 16, 0, 0);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
