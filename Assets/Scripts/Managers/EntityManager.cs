using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityManager : MonoBehaviour
{
    private static EntityManager instance;

    private GameObject player;
    private List<GameObject> enemies;

    private int entranceIndex = -1;
    private string respawnScene = "Temple1";
    private int respawnEntrance = 0;
    private bool isRespawn = false;
    private int playerHpPersist;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        enemies = new List<GameObject>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += TrySetPlayerEntrancePosition;
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= TrySetPlayerEntrancePosition;
        SceneManager.sceneUnloaded -= OnSceneExit;
    }

    public static void RegisterPlayer(GameObject obj)
    {
        instance.player = obj;
    }

    public static void RegisterEnemy(GameObject obj)
    {
        instance.enemies.Add(obj);
    }

    public static void SetPlayerControlEnabled(bool enabled)
    {
        instance.player.GetComponent<CharacterMovement>().SetControlEnabled(enabled);
    }

    public static void SetPlayerAutoChannel(float duration)
    {
        instance.player.GetComponent<CharacterMovement>().AutoChannel(duration);
    }

    public static void FloatPlayerInAir(float duration)
    {
        instance.player.GetComponent<CharacterMovement>().FloatInAir(duration);
    }

    public static void DoPlayerGetAbilityCutscene()
    {
        FloatPlayerInAir(6);
        instance.player.GetComponent<SpriteFlipper>().Spin(10, 2f);
    }

    public static void SetSceneEntrance(int entranceIndex)
    {
        instance.entranceIndex = entranceIndex;
    }

    public static Transform GetPlayerTransform()
    {
        return instance.player.transform;
    }

    public static Vector3 GetPlayerPosition()
    {
        return instance.player.transform.position;
    }

    public static Vector3 GetPlayerStartPosition()
    {
        EntranceManager em = FindObjectOfType<EntranceManager>();
        if (em != null && instance.entranceIndex >= 0)
            return em.entrances[instance.entranceIndex].position;
        return GetPlayerPosition();
    }

    public static int GetPlayerMaxHp()
    {
        return instance.player.GetComponent<CharacterMovement>().GetMaxHitPoints();
    }

    public static int GetPlayerHp()
    {
        return instance.player.GetComponent<CharacterMovement>().GetHitPoints();
    }

    public static bool ObjectIsPlayer(GameObject obj)
    {
        return obj.Equals(instance.player);
    }

    public static void ParentObjectToPlayer(GameObject obj)
    {
        obj.transform.SetParent(instance.player.transform);
    }

    public static void SetRespawnData(string sceneName, int entranceInd)
    {
        instance.respawnEntrance = entranceInd;
        instance.respawnScene = sceneName;
    }

    public static void RespawnPlayer()
    {
        instance.entranceIndex = instance.respawnEntrance;
        instance.isRespawn = true;
        ScreenEffectManager.FadeScreen(Color.black, 0.5f);
        TimingManager.ExecuteAfterDelay(0.5f, instance.DoRespawn);
    }

    public static void PlayerFall()
    {
        CharacterMovement playerMovement = instance.player.GetComponent<CharacterMovement>();
        EntranceManager em = FindObjectOfType<EntranceManager>();
        Vector3 pos = em.entrances[instance.entranceIndex].position;
        float rot = em.entrances[instance.entranceIndex].rotation.y;
        instance.player.transform.position = pos;
        instance.player.GetComponent<SpriteFlipper>().SetFacing(rot != 0);
        playerMovement.StopMovement();
        playerMovement.TrySnapToGround();
        playerMovement.TakeDamage();
    }

    private void DoRespawn()
    {
        SceneManager.LoadScene(instance.respawnScene);
    }

    public static void RecordPlayerHp()
    {
        instance.playerHpPersist = instance.player.GetComponent<CharacterMovement>().GetHitPoints();
    }

    private void OnSceneExit(Scene scene)
    {
        enemies.Clear();
    }


    private void TrySetPlayerEntrancePosition(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetPlayerEntrancePosition(!isRespawn));
    }
    
    private IEnumerator SetPlayerEntrancePosition(bool autoMove = true)
    {
        // Wait for the player character in the new scene to register herself
        while (player == null)
        {
            yield return new WaitForEndOfFrame();
        }

        EntranceManager em = FindObjectOfType<EntranceManager>();
        CharacterMovement playerMovement = player.GetComponent<CharacterMovement>();

        if (em != null && entranceIndex >= 0 && entranceIndex < em.entrances.Count)
        {
            Vector3 pos = em.entrances[entranceIndex].position;
            float rot = em.entrances[entranceIndex].rotation.y;
            player.transform.position = pos;
            player.GetComponent<SpriteFlipper>().SetFacing(rot != 0);
            playerMovement.StopMovement();
            if (autoMove)
            {
                playerMovement.AutoMove(rot != 0, 0.25f);
            }
            isRespawn = false;
        }

        if (playerHpPersist > 0)
        {
            playerMovement.SetHitPoints(playerHpPersist);
        }
        else
        {
            playerMovement.SetHitPoints(playerMovement.GetMaxHitPoints());
        }
        EventManager.InvokeEvent(new PlayerSpawnedEvent());
        yield return null;
    }
}
