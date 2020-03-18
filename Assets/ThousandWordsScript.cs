using System;
using System.Collections;
using System.Collections.Generic;
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

    public TextAsset Lunar;

    public KMSelectable[] Selection;

    public TextMesh[] Displays;
    public TextMesh[] SelectionText;

    private bool[] CoolBool = { false, false, false, false, false };
    private int Tracker = 0;
    private int Theta;
    private bool Playable = false;

    // Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        Selection[0].OnInteract += delegate () { OhYes(); return false; };
        Selection[1].OnInteract += delegate () { OhNo(); return false; };
    }

    void Start()
    {
        Module.OnActivate += NuclearAnswer;
    }

    void NuclearAnswer()
    {
        Playable = true;
        string[] TheAnswer = JsonConvert.DeserializeObject<string[]>(Lunar.text);
        Theta = UnityEngine.Random.Range(0, TheAnswer.Length);
        string ThePoint = TheAnswer[Theta].ToUpper();
        Displays[0].text = ThePoint[0].ToString(); Displays[1].text = ThePoint[1].ToString(); Displays[2].text = ThePoint[2].ToString(); Displays[3].text = ThePoint[3].ToString(); Displays[4].text = ThePoint[4].ToString();
        Debug.LogFormat("[1000 Words #{0}] {1}", moduleId, (Theta + 1).ToString());
    }

    void OhYes()
    {
        int Sell; Sell = UnityEngine.Random.Range(0, 4);
        Audio.PlaySoundAtTransform(SFX[Sell].name, transform);
        Selection[0].AddInteractionPunch(0.2f);
        if (Playable != false)
        {
            if (Theta < 1000)
            {
                CoolBool[Tracker] = true;
            }
            Tracker = Tracker + 1;
            NuclearAnswer();

            if (Tracker == 5)
            {
                Check();
            }
        }
    }

    void OhNo()
    {
        int Steal; Steal = UnityEngine.Random.Range(0, 4);
        Audio.PlaySoundAtTransform(SFX[Steal].name, transform);
        Selection[1].AddInteractionPunch(0.2f);
        if (Playable != false)
        {
            if (Theta > 999)
            {
                CoolBool[Tracker] = true;
            }
            Tracker = Tracker + 1;
            NuclearAnswer();

            if (Tracker == 5)
            {
                Check();
            }
        }
    }

    void Check()
    {
        if ((CoolBool[0] && CoolBool[1] && CoolBool[2] && CoolBool[3] && CoolBool[4]) == true)
        {
            Module.HandlePass();
            int Spell; Spell = UnityEngine.Random.Range(0, 10);
            if (Spell == 0)
            {
                Displays[0].text = "N"; Displays[1].text = "I"; Displays[2].text = "C"; Displays[3].text = "E"; Displays[4].text = "!"; SelectionText[0].text = ""; SelectionText[1].text = "";
            }
            else if (Spell == 1)
            {
                Displays[0].text = "E"; Displays[1].text = "A"; Displays[2].text = "S"; Displays[3].text = "Y"; Displays[4].text = "!"; SelectionText[0].text = ""; SelectionText[1].text = "";
            }
            else if (Spell == 2)
            {
                Displays[0].text = "G"; Displays[1].text = "O"; Displays[2].text = "O"; Displays[3].text = "D"; Displays[4].text = "!"; SelectionText[0].text = ""; SelectionText[1].text = "";
            }
            else if (Spell == 3)
            {
                Displays[0].text = ""; Displays[1].text = "G"; Displays[2].text = "G"; Displays[3].text = "!"; Displays[4].text = ""; SelectionText[0].text = ""; SelectionText[1].text = "";
            }
            else if (Spell == 4)
            {
                Displays[0].text = ""; Displays[1].text = ""; Displays[2].text = ""; Displays[3].text = ""; Displays[4].text = ""; SelectionText[0].text = "EZ!"; SelectionText[1].text = "PZ!";
            }
            else if (Spell == 5)
            {
                Displays[0].text = "G"; Displays[1].text = "R"; Displays[2].text = "E"; Displays[3].text = "A"; Displays[4].text = "T"; SelectionText[0].text = ""; SelectionText[1].text = "";
            }
            else if (Spell == 6)
            {
                Displays[0].text = "N"; Displays[1].text = "E"; Displays[2].text = "A"; Displays[3].text = "T"; Displays[4].text = "!"; SelectionText[0].text = ""; SelectionText[1].text = "";
            }
            else if (Spell == 7)
            {
                Displays[0].text = "!"; Displays[1].text = "!"; Displays[2].text = "!"; Displays[3].text = "!"; Displays[4].text = "!"; SelectionText[0].text = ""; SelectionText[1].text = "";
            }
            else if (Spell == 8)
            {
                Displays[0].text = "G"; Displays[1].text = "R"; Displays[2].text = "E"; Displays[3].text = "A"; Displays[4].text = "T"; SelectionText[0].text = "JOB"; SelectionText[1].text = "MAN";
            }
            else if (Spell == 9)
            {
                Displays[0].text = "C"; Displays[1].text = "L"; Displays[2].text = "E"; Displays[3].text = "A"; Displays[4].text = "R"; SelectionText[0].text = "!!!!!"; SelectionText[1].text = "!!!!!";
            }
            Audio.PlaySoundAtTransform(SFX[4].name, transform);
            Playable = false;
        }

        else
        {
            StartCoroutine(Developed());
        }
    }

    IEnumerator Developed()
    {
        Playable = false;
        int Spell; Spell = UnityEngine.Random.Range(0, 10);
        if (Spell == 0)
        {
            Displays[0].text = "S"; Displays[1].text = "R"; Displays[2].text = "S"; Displays[3].text = "L"; Displays[4].text = "Y"; SelectionText[0].text = ""; SelectionText[1].text = "";
        }
        else if (Spell == 1)
        {
            Displays[0].text = "W"; Displays[1].text = "H"; Displays[2].text = "A"; Displays[3].text = "T"; Displays[4].text = "?"; SelectionText[0].text = ""; SelectionText[1].text = "";
        }
        else if (Spell == 2)
        {
            Displays[0].text = "B"; Displays[1].text = "R"; Displays[2].text = "U"; Displays[3].text = "H"; Displays[4].text = "!"; SelectionText[0].text = ""; SelectionText[1].text = "";
        }
        else if (Spell == 3)
        {
            Displays[0].text = "N"; Displays[1].text = "O"; Displays[2].text = "P"; Displays[3].text = "E"; Displays[4].text = "!"; SelectionText[0].text = ""; SelectionText[1].text = "";
        }
        else if (Spell == 4)
        {
            Displays[0].text = ""; Displays[1].text = ""; Displays[2].text = ""; Displays[3].text = ""; Displays[4].text = ""; SelectionText[0].text = "NO!"; SelectionText[1].text = "NO!";
        }
        else if (Spell == 5)
        {
            Displays[0].text = "H"; Displays[1].text = "M"; Displays[2].text = "M"; Displays[3].text = "M"; Displays[4].text = "M"; SelectionText[0].text = ""; SelectionText[1].text = "";
        }
        else if (Spell == 6)
        {
            Displays[0].text = ""; Displays[1].text = "-"; Displays[2].text = "_"; Displays[3].text = "-"; Displays[4].text = ""; SelectionText[0].text = ""; SelectionText[1].text = "";
        }
        else if (Spell == 7)
        {
            Displays[0].text = "?"; Displays[1].text = "?"; Displays[2].text = "?"; Displays[3].text = "?"; Displays[4].text = "?"; SelectionText[0].text = ""; SelectionText[1].text = "";
        }
        else if (Spell == 8)
        {
            Displays[0].text = ""; Displays[1].text = ""; Displays[2].text = ""; Displays[3].text = ""; Displays[4].text = ""; SelectionText[0].text = "OH"; SelectionText[1].text = "BOY";
        }
        else if (Spell == 9)
        {
            Displays[0].text = "R"; Displays[1].text = "E"; Displays[2].text = "S"; Displays[3].text = "E"; Displays[4].text = "T"; SelectionText[0].text = "!!!!!"; SelectionText[1].text = "!!!!!";
        }
        Audio.PlaySoundAtTransform(SFX[5].name, transform);
        yield return new WaitForSecondsRealtime(1.5f);
        Module.HandleStrike();
        CoolBool[0] = false; CoolBool[1] = false; CoolBool[2] = false; CoolBool[3] = false; CoolBool[4] = false;
        Tracker = 0;
        SelectionText[0].text = "YES"; SelectionText[1].text = "NO";
        NuclearAnswer();
    }
}
