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
    private int corrAns;
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
        Debug.LogFormat("[1000 Words #{0}] <Stage {2}> The displayed word is: {1}.", moduleId, AllWords[WordIndex], Stage+1);
        if (WordIndex < 1000)
            corrAns = 0;
        else
            corrAns = 1;
        Debug.LogFormat("[1000 Words #{0}] <Stage {2}> {3} therefore you need to press the '{1}' button.", moduleId, corrAns == 0 ? "YES" : "NO", Stage+1, corrAns == 0 ? "The word is in the manual's table," : "The word is not in the manual's table,");
    }

    void PressButton(KMSelectable Button)
    {
        int ix = Array.IndexOf(Buttons, Button);
        Audio.PlaySoundAtTransform(SFX[UnityEngine.Random.Range(0, 4)].name, Button.transform);
        Button.AddInteractionPunch(.2f);
        if (Playable && !ModuleSolved)
        {
            Debug.LogFormat("[1000 Words #{0}] <Stage {2}> You pressed the '{1}' button, moving to the next stage.", moduleId, ButtonTexts[ix].text, Stage+1);
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
            Debug.LogFormat("[1000 Words #{0}] All 5 stages were correct. Module solved!", moduleId);
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
        string wrongstages = "";
        for (int i = 0; i < 5; i++)
        {
            if (!Submitted[i])
                wrongstages += (i+1)+" ";
        }
        wrongstages = wrongstages.Trim();
        Debug.LogFormat("[1000 Words #{0}] The following stages had incorrect inputs: {1}, therefore a strike has been given. Resetting back to stage 1...", moduleId, wrongstages);
        for (int i = 0; i < 5; i++)
            Submitted[i] = false;
        Stage = 0;
        ButtonTexts[0].text = "YES"; ButtonTexts[1].text = "NO";
        GenerateAnswer();
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} yes/y [Presses the 'YES' button] | !{0} no/n [Presses the 'NO' button]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*yes\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*y\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Buttons[0].OnInteract();
            if (Stage == 5 && Submitted.Contains(false))
            {
                yield return "strike";
            }
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*no\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*n\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Buttons[1].OnInteract();
            if (Stage == 5 && Submitted.Contains(false))
            {
                yield return "strike";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        int start = Stage;
        if (start > 0)
        {
            Debug.LogFormat("[1000 Words {0}] Twitch Plays Autosolver: At least one of the previously inputted stages is incorrect, resetting...");
            for (int i = 0; i < start; i++)
            {
                if (Submitted[i] == false)
                {
                    start = 0;
                    Stage = 0;
                    Submitted[0] = false;
                    Submitted[1] = false;
                    Submitted[2] = false;
                    Submitted[3] = false;
                    Submitted[4] = false;
                    GenerateAnswer();
                    break;
                }
            }
        }
        for (int i = start; i < 5; i++)
        {
            Buttons[corrAns].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
