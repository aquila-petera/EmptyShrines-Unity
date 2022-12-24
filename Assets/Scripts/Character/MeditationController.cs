using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeditationController : MonoBehaviour
{
    [SerializeField]
    private TextLabel prompt;
    [SerializeField]
    private TMP_Text leftChoice;
    [SerializeField]
    private TMP_Text rightChoice;

    private static int promptCount;
    private static List<TextLabelManager.Glyph> glyphs;
    private Animator animator;
    private bool leftCorrect;

    public static void SetPromptData(List<TextLabelManager.Glyph> glyphPool, int promptNumber)
    {
        glyphs.Clear();
        glyphs.AddRange(glyphPool.ToArray());
        promptCount = promptNumber;
    }

    public void GenerateNewPrompt()
    {
        int idxMain = Random.Range(0, glyphs.Count);
        int idxOther = (idxMain > 0 && Random.Range(0, 2) == 1) || idxMain == glyphs.Count - 1 ? Random.Range(0, idxMain) : Random.Range(idxMain + 1, glyphs.Count);
        TextLabelManager.Glyph glyph = glyphs[idxMain];
        TextLabelManager.Glyph other = glyphs[idxOther];
        prompt.SetBaseString(glyph.engChars);
        if (Random.Range(0, 2) == 1)
        {
            leftChoice.text = glyph.engChars;
            rightChoice.text = other.engChars;
            leftCorrect = true;
        }
        else
        {
            leftChoice.text = other.engChars;
            rightChoice.text = glyph.engChars;
            leftCorrect = false;
        }
        glyphs.RemoveAt(idxMain);
    }

    public void Choose(bool left)
    {
        if (leftCorrect == left)
        {
            ParticleManager.PlayParticleSystemAtPosition("Dots", EntityManager.GetPlayerPosition(), Color.yellow);
            if (--promptCount > 0)
            {
                animator.Rebind();
                GenerateNewPrompt();
            }
            else
            {
                EventManager.InvokeEvent(new PlayerFinishedMeditatingEvent(true));
            }
        }
        else
        {
            EventManager.InvokeEvent(new PlayerFinishedMeditatingEvent(false));
            ParticleManager.PlayParticleSystemAtPosition("Dots", EntityManager.GetPlayerPosition(), Color.red);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        glyphs = new List<TextLabelManager.Glyph>();
        gameObject.SetActive(false);
    }
}
