using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolStatue : BaseInteractable
{
    [Serializable]
    private struct GlyphRange
    {
        public int startIdx, endIdx;
    }
    // 1-9 a
    // 14-20 i

    [SerializeField]
    private List<GlyphRange> glyphRanges;
    [SerializeField]
    private int promptCount;
    [SerializeField]
    private List<GameObject> triggeredObjects;
    [SerializeField]
    private ParticleSystem particles;

    private bool active = true;

    public override void OnInteract()
    {
        if (!active)
            return;

        var allGlyphs = TextLabelManager.GetAllGlyphs();
        List<TextLabelManager.Glyph> subset = new List<TextLabelManager.Glyph>();
        foreach (GlyphRange range in glyphRanges)
        {
            for (int i = range.startIdx; i <= range.endIdx; i++)
            {
                subset.Add(allGlyphs[i]);
            }
        }
        MeditationController.SetPromptData(subset, promptCount);
        StopCoroutine("ShowHint");
        StartCoroutine(HideHint());
        active = false;
        player.ToggleMeditate(true);
    }

    new public void OnTriggerEnter(Collider col)
    {
        if (active)
        {
            base.OnTriggerEnter(col);
        }
    }

    new public void OnTriggerExit(Collider col)
    {
        if (active)
        {
            base.OnTriggerExit(col);
        }
    }

    private void Start()
    {
        EventManager.BindEvent(typeof(PlayerFinishedMeditatingEvent), OnMeditationEnd);
    }

    private void OnDestroy()
    {
        EventManager.UnbindEvent(typeof(PlayerFinishedMeditatingEvent), OnMeditationEnd);
    }

    private void OnMeditationEnd(BasicEvent e)
    {
        if ((e as PlayerFinishedMeditatingEvent).success)
        {
            active = false;
            particles.Stop();
            foreach (GameObject obj in triggeredObjects)
            {
                obj.GetComponent<IActivatable>().Activate();
            }
        }
        else
        {
            active = true;
            StopCoroutine("HideHint");
            StartCoroutine(ShowHint());
        }
    }
}
