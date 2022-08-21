using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [System.Serializable]
    private struct EnemyWave
    {
        public List<BasicEnemy> enemies;
    }

    [SerializeField]
    private List<EnemyWave> enemyWaves;

    [SerializeField]
    private List<GameObject> arenaBarriers;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private AudioClip battleBGM;

    private bool combatStarted = false;
    private int currentWave;
    private int enemiesLeft;

    private void Start()
    {
        EventManager.BindEvent(typeof(EnemyKOEvent), OnEnemyKO);
    }

    protected void OnDestroy()
    {
        EventManager.UnbindEvent(typeof(EnemyKOEvent), OnEnemyKO);
    }

    public void StartCombat()
    {
        if (combatStarted)
            return;

        combatStarted = true;
        MusicManager.PlayCustomBGM(battleBGM, 0, 0.4f);
        CameraManager.LockCamera(cameraTransform);
        foreach (GameObject obj in arenaBarriers)
        {
            obj.GetComponent<IActivatable>().Activate();
        }
        TimingManager.ExecuteAfterDelay(0.5f, SpawnNextWave);
    }

    private void EndCombat()
    {
        MusicManager.PlayMapBGM(0, 0.4f);
        CameraManager.UnlockCamera();
        foreach (GameObject obj in arenaBarriers)
        {
            obj.GetComponent<IActivatable>().Activate();
        }
    }

    private void SpawnNextWave()
    {
        if (currentWave > enemyWaves.Count - 1)
        {
            EndCombat();
            return;
        }

        EnemyWave wave = enemyWaves[currentWave];
        enemiesLeft = wave.enemies.Count;
        foreach (BasicEnemy enemy in wave.enemies)
        {
            enemy.gameObject.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (EntityManager.ObjectIsPlayer(col.gameObject))
        {
            StartCombat();
        }
    }

    private void OnEnemyKO(BasicEvent ev)
    {
        EnemyKOEvent ekoEvent = ev as EnemyKOEvent;
        if (enemyWaves[currentWave].enemies.Contains(ekoEvent.enemy))
        {
            enemiesLeft--;
            if (enemiesLeft <= 0)
            {
                currentWave++;
                TimingManager.ExecuteAfterDelay(0.5f, SpawnNextWave);
            }
        }
    }
}
