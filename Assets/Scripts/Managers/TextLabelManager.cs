using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextLabelManager : MonoBehaviour
{
    private static TextLabelManager instance;

    public Action<string> OnKeystroke;
    public Action<string> OnSubmit;

    [Serializable]
    struct Glyph
    {
        public string glyphChars;
        public string engChars;
    }

    [SerializeField]
    private List<Glyph> glyphs;

    private Dictionary<string, string> engToGlyph;
    private List<TextLabel> labels;

    public static void RegisterLabel(TextLabel label)
    {
        instance.labels.Add(label);
        instance.OnKeystroke += label.UpdateHighlight;
        instance.OnSubmit += label.Activate;
    }

    public static void UnregisterLabel(TextLabel label)
    {
        instance.labels.Remove(label);
        instance.OnKeystroke -= label.UpdateHighlight;
        instance.OnSubmit -= label.Activate;
    }

    public static void UpdateChannelString(string typeStr)
    {
        instance.OnKeystroke?.Invoke(typeStr);
    }

    public static void SubmitChannelString(string typeStr)
    {
        instance.OnSubmit?.Invoke(typeStr);
    }

    public static string EngToGlyph(string eng)
    {
        if (eng == null)
            return "";
        string temp = "";
        string ret = "";
        foreach (char c in eng)
        {
            temp += c;
            if (instance.engToGlyph.ContainsKey(temp))
            {
                string glyph = "";
                instance.engToGlyph.TryGetValue(temp, out glyph);
                ret += glyph;
                temp = "";
            }
        }
        return ret;
    }

    public static void StartTranslation()
    {
        foreach (TextLabel label in instance.labels)
        {
            label.StartTranslation();
        }
    }

    public static void StopTranslation()
    {
        foreach (TextLabel label in instance.labels)
        {
            label.StopTranslation();
        }
    }

    public string EngToGlyph_GUI(string eng)
    {
        if (eng == null)
            return "";
        string temp = "";
        string ret = "";
        foreach (char c in eng)
        {
            temp += c;
            foreach (Glyph glyph in glyphs)
            {
                if (glyph.engChars == temp)
                {
                    ret += glyph.glyphChars;
                    temp = "";
                }
            }
        }
        return ret;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        labels = new List<TextLabel>();
        engToGlyph = new Dictionary<string, string>();
        foreach (Glyph glyph in glyphs)
        {
            engToGlyph.Add(glyph.engChars, glyph.glyphChars);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneExit;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneExit;
    }

    private void OnSceneExit(Scene arg0)
    {
        labels.Clear();
    }

}
