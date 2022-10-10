using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextLabel : MonoBehaviour
{
    [SerializeField]
    private string baseString;
    [SerializeField]
    private List<GameObject> triggeredObjects;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float hiddenRange;
    [SerializeField]
    private bool oneTimeUse;
    [SerializeField]
    private bool autoActivateRight;
    [SerializeField]
    private bool startDisabled;
    [SerializeField]
    private TMP_Text translation;
    [SerializeField]
    private AudioClip chimeSound;

    private TMP_Text highlight;

    private GameObject highlightObj;

    private TMP_Text text;
    new private Renderer renderer;
    private Material materialInstance;
    private float animParam;
    private float translateTimer;
    private bool activeTimerRunning;
    private float targetAlpha = 1;
    private Color baseOutlineColor;
    private ParticleSystem particles;
    private float translationTime = 1f;

    public void UpdateHighlight(string typeStr)
    {
        if (!renderer.enabled || !renderer.isVisible || activeTimerRunning || !IsVisible() || !IsIdle())
        {
            return;
        }

        if (baseString.StartsWith(typeStr))
        {
            highlight.text = TextLabelManager.EngToGlyph(typeStr);
        }
        else
        {
            highlight.text = "";
        }
    }

    private void ForceActivate()
    {
        AudioSource.PlayClipAtPoint(chimeSound, transform.position);
        if (duration > 0)
        {
            StartCoroutine(DoActiveTimer());
        }
        ParticleManager.PlayParticleSystemAtPosition("Stars", transform.position);
        ActivateTriggers();
        if (oneTimeUse)
        {
            Destroy(gameObject);
        }
    }

    public void Activate(string typeStr)
    {
        highlight.text = "";
        if (!renderer.isVisible || activeTimerRunning || !IsVisible() || !IsIdle())
        {
            return;
        }

        if (typeStr == baseString)
        {
            ForceActivate();
        }
    }

    public void AddTriggeredObject(GameObject obj)
    {
        triggeredObjects.Add(obj);
    }

    public void StartTranslation()
    {
        if (IsVisible())
        {
            translateTimer = translationTime;
            highlight.text = text.text;
        }
    }

    public void StopTranslation()
    {
        translateTimer = 0;
        highlight.text = "";
        translation.text = "";
    }

    public void ForceDeactivate()
    {
        if (activeTimerRunning)
        {
            ActivateTriggers();
            activeTimerRunning = false;
            highlight.text = "";
        }
    }

    public void SetEnabled(bool enabled)
    {
        renderer.enabled = enabled;
    }

    public bool GetEnabled()
    {
        return renderer.enabled;
    }

    private void ActivateTriggers()
    {
        foreach (GameObject obj in triggeredObjects)
        {
            obj.GetComponent<IActivatable>().Activate();
        }
    }

    private IEnumerator DoActiveTimer()
    {
        activeTimerRunning = true;
        float d = duration;
        while (d > 0)
        {
            d -= Time.deltaTime;
            if (d > 6)
                highlight.text = Mathf.Round(d * 10) % 10 == 0 ? "" : text.text;
            else if (d > 2)
                highlight.text = Mathf.Round(d * 10) % 5 == 0 ? "" : text.text;
            else
                highlight.text = Mathf.Round(d * 10) % 2 == 0 ? "" : text.text;
            yield return new WaitForEndOfFrame();
        }
        ParticleManager.PlayParticleSystemAtPosition("Stars", transform.position);
        ActivateTriggers();
        activeTimerRunning = false;
        highlight.text = "";
        yield return null;
    }

    private void OnEnable()
    {
        if (highlightObj != null)
        {
            highlightObj.GetComponent<Renderer>().material = materialInstance;
        }
    }

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        particles = GetComponent<ParticleSystem>();
        text.text = TextLabelManager.EngToGlyph(baseString);
        baseOutlineColor = text.outlineColor;

        highlightObj = Instantiate(gameObject, transform.parent);

        highlightObj.transform.SetParent(transform);
        highlightObj.GetComponent<TextLabel>().enabled = false;
        Destroy(highlightObj.GetComponent<ParticleSystem>());
        highlightObj.transform.Translate(0, 0, -0.0001f);
        highlight = highlightObj.GetComponent<TMP_Text>();
        highlight.text = "";

        translation.text = "";

        TextLabelManager.RegisterLabel(this);
        renderer = GetComponent<Renderer>();
        materialInstance = Instantiate(renderer.material);
        highlightObj.GetComponent<Renderer>().material = materialInstance;

        if (hiddenRange > 0)
        {
            text.alpha = 0;
        }

        if (startDisabled)
        {
            SetEnabled(false);
        }

        EventManager.BindEvent(typeof(PlayerSpawnedEvent), OnPlayerSpawned);
    }

    private void Update()
    {
        if (translateTimer > 0)
        {
            translateTimer -= Time.deltaTime;
            if (translateTimer <= 0)
            {
                ParticleManager.PlayParticleSystemAtPosition("Stars", transform.position, Color.blue);
                translation.text = baseString;
            }
        }

        animParam += Mathf.PI * 2 * Time.deltaTime;
        if (animParam >= Mathf.PI)
            animParam -= Mathf.PI;

        if (!activeTimerRunning && IsVisible())
        {
            materialInstance.SetColor("_OutlineColor", translateTimer > 0 || translation.text != "" ? Color.blue : Color.yellow);
            materialInstance.SetFloat("_OutlineWidth", 0.1f + Mathf.Sin(animParam) * 0.2f);
            translation.color = Color.Lerp(Color.white, Color.blue, Mathf.Sin(animParam));
        }

        if (hiddenRange > 0)
        {
            float dist = Vector2.Distance(transform.position, EntityManager.GetPlayerPosition());
            if (dist == 0)
            {
                targetAlpha = 1;
            }
            else if (dist <= hiddenRange)
            {
                targetAlpha = 0.5f + (0.2f / dist) * 0.5f;
            }
            else if (dist > hiddenRange)
            {
                targetAlpha = 0;
            }

            if (targetAlpha > 0 && particles.isPlaying)
            {
                particles.Stop();
            }
            else if (targetAlpha == 0 && !particles.isPlaying)
            {
                particles.Play();
            }
            
            if (Mathf.Abs(text.alpha - targetAlpha) > 0.01f)
            {
                text.alpha = Mathf.Lerp(text.alpha, targetAlpha, 5 * Time.deltaTime);
            }
            else
            {
                text.alpha = targetAlpha;
            }    
        }
    }

    private void OnPlayerSpawned(BasicEvent e)
    {
        if (autoActivateRight && EntityManager.GetPlayerStartPosition().x > transform.position.x)
        {
            autoActivateRight = false;
            ForceActivate();
        }
    }
    private void OnDestroy()
    {
        EventManager.UnbindEvent(typeof(PlayerSpawnedEvent), OnPlayerSpawned);
        TextLabelManager.UnregisterLabel(this);
    }

    private bool IsIdle()
    {
        bool idle = true;
        foreach (var obj in triggeredObjects)
        {
            if (obj.GetComponent<IActivatable>().IsActive())
            {
                idle = false;
            }
        }    
        return idle;
    }

    private bool IsVisible()
    {
        return renderer.isVisible && text.alpha > 0 && targetAlpha > 0;
    }

    public void SetBaseString(string str)
    {
        baseString = str;
        UpdateText();
    }

    public void UpdateText()
    {
        var tempText = GetComponent<TMP_Text>();
        GameObject managerObj = Resources.Load<GameObject>("Prefabs/Managers/GameManager");
        TextLabelManager manager = managerObj.GetComponent<TextLabelManager>();
        tempText.text = manager.EngToGlyph_GUI(baseString);
        tempText.ForceMeshUpdate();
    }
}
