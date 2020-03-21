using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using KModkit;

public class ThousandWordsScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    public AudioClip[] SFX;

    public TextAsset WordBank;

    public KMSelectable[] Buttons;

    public TextMesh[] Displays;
    public TextMesh[] ButtonTexts;

    private bool[] Submitted = { false, false, false, false, false };
    private int Stage = 0;
    private int WordIndex;
    private bool Playable = false;

    // Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable Button in Buttons)
            Button.OnInteract += delegate () { PressButton(Button); return false; };
        Module.OnActivate += GenerateAnswer;
    }

    void GenerateAnswer()
    {
        Playable = true;
        string[] AllWords = JsonConvert.DeserializeObject<string[]>(WordBank.text);
        WordIndex = UnityEngine.Random.Range(0, AllWords.Length);
        string Uppercase = AllWords[WordIndex].ToUpperInvariant();
        for (int i = 0; i < 5; i++)
            Displays[i].text = Uppercase[i].ToString();
        Debug.LogFormat("[1000 Words #{0}] The display says: {1}.", moduleId, AllWords[WordIndex]);
        Debug.LogFormat("[1000 Words #{0}] You need to press: {1}.", moduleId, WordIndex < 1000 ? "YES" : "NO");
    }

    void PressButton(KMSelectable Button)
    {
        int ix = Array.IndexOf(Buttons, Button);
        Audio.PlaySoundAtTransform(SFX[UnityEngine.Random.Range(0, 4)].name, transform);
        Button.AddInteractionPunch(.2f);
        if (Playable && !ModuleSolved)
        {
            Debug.LogFormat("[1000 Words #{0}] You pressed {1}.", moduleId, ButtonTexts[ix].text);
            if (ix == 0 ? (WordIndex < 1000) : (WordIndex > 999))
                Submitted[Stage] = true;
            Stage++;
            if (Stage == 5)
                AnswerCheck();
            else
                GenerateAnswer();
        }
    }

    void AnswerCheck()
    {
        if (!Submitted.Contains(false))
        {
            Module.HandlePass();
            ModuleSolved = true;
            Debug.LogFormat("[1000 Words #{0}] Module solved!", moduleId);
            int MessageIndex = UnityEngine.Random.Range(0, 10);
            switch (MessageIndex)
            {
                case 4:
                    ButtonTexts[0].text = "EZ!";
                    ButtonTexts[1].text = "PZ!";
                    break;
                case 8:
                    ButtonTexts[0].text = "JOB";
                    ButtonTexts[1].text = "MAN";
                    break;
                case 9:
                    foreach (TextMesh ButtonText in ButtonTexts)
                        ButtonText.text = "!!!!!";
                    break;
                default:
                    foreach (TextMesh ButtonText in ButtonTexts)
                        ButtonText.text = "";
                    break;
            }
            string[] Messages = new string[] { "NICE!", "EASY!", "GOOD!", " GG! ", "     ", "GREAT", "NEAT!", "!!!!!", "GREAT", "CLEAR" };
            for (int i = 0; i < 5; i++)
                Displays[i].text = Messages[MessageIndex][i].ToString();
            Audio.PlaySoundAtTransform(SFX[4].name, transform);
            Playable = false;
        }
        else
            StartCoroutine(StrikeAnimation());
    }

    IEnumerator StrikeAnimation()
    {
        Playable = false;
        int MessageIndex = UnityEngine.Random.Range(0, 10);
        switch (MessageIndex)
        {
            case 4:
                foreach (TextMesh ButtonText in ButtonTexts)
                    ButtonText.text = "NO!";
                break;
            case 8:
                ButtonTexts[0].text = "OH";
                ButtonTexts[1].text = "BOY";
                break;
            case 9:
                foreach (TextMesh ButtonText in ButtonTexts)
                    ButtonText.text = "!!!!!";
                break;
            default:
                foreach (TextMesh ButtonText in ButtonTexts)
                    ButtonText.text = "";
                break;
        }
        string[] Messages = new string[] { "SRSLY", "WHAT?", "BRUH!", "NOPE!", "     ", "HMMMM", " -_- ", "?????", "    ", "RESET" };
        for (int i = 0; i < 5; i++)
            Displays[i].text = Messages[MessageIndex][i].ToString();
        Audio.PlaySoundAtTransform(SFX[5].name, transform);
        yield return new WaitForSecondsRealtime(1.5f);
        Module.HandleStrike();
        Debug.LogFormat("[1000 Words #{0}] Strike! Resetting...", moduleId);
        for (int i = 0; i < 5; i++)
            Submitted[i] = false;
        Stage = 0;
        ButtonTexts[0].text = "YES"; ButtonTexts[1].text = "NO";
        GenerateAnswer();
    }
}
