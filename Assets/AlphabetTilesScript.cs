using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class AlphabetTilesScript : MonoBehaviour
{
	public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;
	
	public KMSelectable[] MainButtons;
	public TextMesh[] Alphabet;
	public TextMesh Basis;
	public KMSelectable UtilityButton, MusicButton;
	public SpriteRenderer[] UtilityImages;
	public Sprite[] Sprites;
	public MeshRenderer[] Center, Border;
	public Material[] Colors;
	public AudioClip[] SFX;
	public GameObject HL, HL2, Loading;
	public AudioSource Audio2;
	
	string MainAnswer;
	string[] LettersShown = new string[6];
	string[] RegularAlphabet = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
	string[] ShuffledAlphabet = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
	Coroutine Cycle;
	
	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;
	
	void Awake()
	{
		moduleId = moduleIdCounter++;
		for (int a = 0; a < MainButtons.Count(); a++)
        {
            int Numbered = a;
            MainButtons[Numbered].OnInteract += delegate
            {
                Press(Numbered);
				return false;
            };
        }
		UtilityButton.OnInteract += delegate () { UtilityPress(); return false; };
		MusicButton.OnInteract += delegate () { MusicPress(); return false; };
	}
	
	void Start()
	{
		GenerateASequence();
	}
	
	void Update()
	{
		if (UtilityImages[0].sprite == Sprites[2])
		{
			Loading.transform.Rotate(-Vector3.forward * 3);
		}
	}
	
	void Press(int Numbered)
	{
		if (UtilityImages[0].sprite == Sprites[2])
		{
			StopCoroutine(Cycle);
			if (Alphabet[Numbered].text == MainAnswer)
			{	
				Basis.text = "!";
				Debug.LogFormat("[Alphabet Tiles #{0}] Correct letter chosen. Module solved!", moduleId);
				StartCoroutine(Winner());
			}
			
			else
			{
				Basis.text = "-";
				Debug.LogFormat("[Alphabet Tiles #{0}] Incorrect letter chosen. A strike is going to be given", moduleId);
				StartCoroutine(AMistake());
			}
		}
		
		
	}
	
	void UtilityPress()
	{
		if (UtilityImages[0].sprite == Sprites[0])
		{
			Cycle = StartCoroutine(LetterCycles());
		}
		else if (UtilityImages[0].sprite == Sprites[1])
		{
			HL2.SetActive(false);
			HL.SetActive(false);
			StopCoroutine(Cycle);
			Cycle = StartCoroutine(Timer());
		}
	}
	
	void MusicPress()
	{
		if (UtilityImages[0].sprite == Sprites[1])
		{
			UtilityImages[1].sprite = UtilityImages[1].sprite == Sprites[4] ? Sprites[3] : Sprites[4];
		}
	}
	
	void GenerateASequence()
	{
		HL.SetActive(false);
		ShuffledAlphabet.Shuffle();
		int NumberVariable = Array.IndexOf(RegularAlphabet, ShuffledAlphabet[25]);
		string CycleIfProper = "";
		foreach (TextMesh Letter in Alphabet)
		{
			int Number = Array.IndexOf(Alphabet, Letter);
			Letter.text = ShuffledAlphabet[Number];
		}
		Basis.text = ShuffledAlphabet[UnityEngine.Random.Range(0,25)];
		for (int x = 0; x < 6; x++)
		{
			LettersShown[x] = ShuffledAlphabet[UnityEngine.Random.Range(0,25)];
			int Following1 = x == 0 ? Array.IndexOf(ShuffledAlphabet, Basis.text) : Array.IndexOf(ShuffledAlphabet, RegularAlphabet[NumberVariable]);
			int Following2 = Array.IndexOf(ShuffledAlphabet, LettersShown[x]);			
			int NumberAdded = 0;
				
			if (Following1 == Following2)
			{
				NumberAdded = 25;
			}
			
			else if (Following1 < Following2)
			{
				NumberAdded = Following2 - Following1;
			}
			
			else 
			{
				NumberAdded = 25 - (Following1 - Following2);
			}
			
			do
			{
				NumberVariable = (NumberVariable + NumberAdded) % 26;
			}
			while (RegularAlphabet[NumberVariable] == ShuffledAlphabet[25]);
			if (x == 0) CycleIfProper += ShuffledAlphabet[25];
			CycleIfProper += " -> " + RegularAlphabet[NumberVariable];
		}
		
		string Sequence = "";
		Debug.LogFormat("[Alphabet Tiles #{0}] Tile Formation: ", moduleId);
		for (int a = 0; a < 5; a++)
		{
			for (int b = 0; b < 5; b++)
			{
				Sequence += b < 4 ? ShuffledAlphabet[(a*5) + b] + ", " : ShuffledAlphabet[(a*5) + b];
			}
			Debug.LogFormat("[Alphabet Tiles #{0}] {1} ", moduleId, Sequence);
			Sequence = "";
		}
		Sequence = "Cycle Sequence: ";
		
		for (int c = 0; c < 6; c++)
		{
			Sequence += c < 5 ? LettersShown[c] + ", " : LettersShown[c];
		}
		MainAnswer = RegularAlphabet[NumberVariable];
		Debug.LogFormat("[Alphabet Tiles #{0}] Starting Letter: {1} - Missing Letter: {2}", moduleId, Basis.text, ShuffledAlphabet[25]);
		Debug.LogFormat("[Alphabet Tiles #{0}] {1} ", moduleId, Sequence);
		Debug.LogFormat("[Alphabet Tiles #{0}] Sequence To The Answer If Done Properly (Starting From The Initial Letter): {1} ", moduleId, CycleIfProper);
		Debug.LogFormat("[Alphabet Tiles #{0}] The Answer: {1}", moduleId, MainAnswer);
	}
	
	IEnumerator LetterCycles()
	{
		foreach (TextMesh Letter in Alphabet)
		{
			Letter.text = "";
		}
		UtilityImages[0].sprite = Sprites[1];
		UtilityImages[1].sprite = Sprites[3];
		Basis.text = "";
		HL.SetActive(true);
		
		while (true)
		{	
			for (int x = 0; x < LettersShown.Length; x++)
			{
				for (int i = 0; i < Center.Length; i++)
				{
					Center[i].material = Colors[2];
					Border[i].material = Colors[0];
				}
				if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[x % 2 == 0 ? 0 : 3].name, transform);
				yield return new WaitForSecondsRealtime(0.2f);
			
				int[][] PixelEstimatedView = new int[26][]{
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1}, //A
					new int[25] {1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0}, //B
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1}, //C
					new int[25] {1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0}, //D
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1}, //E
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0}, //F
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1}, //G
					new int[25] {1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1}, //H
					new int[25] {1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1}, //I
					new int[25] {1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0}, //J
					new int[25] {1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0}, //K
					new int[25] {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0}, //L
					new int[25] {1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1}, //M
					new int[25] {1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1}, //N
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1}, //O
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0}, //P
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1}, //Q
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1}, //R
					new int[25] {1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1}, //S
					new int[25] {1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0}, //T
					new int[25] {1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1}, //U
					new int[25] {1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0}, //V
					new int[25] {1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1}, //W
					new int[25] {1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1}, //X
					new int[25] {1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0}, //Y
					new int[25] {1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1}, //Z
				};
				
				int SelectedLetter = Array.IndexOf(RegularAlphabet, LettersShown[x]);
				for (int i = 0; i < Center.Length; i++)
				{
					Center[i].material = Colors[PixelEstimatedView[SelectedLetter][i]];
					Border[i].material = Colors[PixelEstimatedView[SelectedLetter][i]];
				}
				if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[x % 2 == 0 ? 1 : 2].name, transform);
				yield return new WaitForSecondsRealtime(0.8f);
			}
			
			int[] SelectedNumbers = {0, 4, 20, 24};
			for (int a = 0; a < Center.Length; a++)
			{
				Center[a].material = Colors[0];
				Border[a].material = Colors[0];
				
			}
			
			for (int b = 0; b < SelectedNumbers.Length; b++)
			{
				Center[SelectedNumbers[b]].material = Colors[1];
				Border[SelectedNumbers[b]].material = Colors[1];
				
				switch (b)
				{
					case 0:
						if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[2].name, transform);
						break;
					case 1:
						if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[0].name, transform);
						break;
					case 2:
						if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[3].name, transform);
						break;
					case 3:
						if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[1].name, transform);
						break;
					default:
						break;
				}
				
				yield return new WaitForSecondsRealtime(b % 2 == 0 ? 0.25f : 0.3f);
			}
			
			yield return new WaitForSecondsRealtime(0.3f);
			for (int c = 0; c < Center.Length; c++)
			{
				Center[c].material = !c.EqualsAny(0, 4, 20, 24) ? Colors[1] : Colors[0];
				Border[c].material = !c.EqualsAny(0, 4, 20, 24) ? Colors[1] : Colors[0];
			}
			if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[4].name, transform);
			yield return new WaitForSecondsRealtime(0.2f);
			for (int d = 0; d < Center.Length; d++)
			{
				Center[d].material = !d.EqualsAny(0, 4, 20, 24) ? Colors[0] : Colors[1];
				Border[d].material = !d.EqualsAny(0, 4, 20, 24) ? Colors[0] : Colors[1];
			}
			if (UtilityImages[1].sprite == Sprites[3]) Audio.PlaySoundAtTransform(SFX[5].name, transform);
			yield return new WaitForSecondsRealtime(0.3f);
		}
	}
	
	IEnumerator Timer()
	{
		PlayTime = true;
		for (int x = 0; x < Center.Length; x++)
		{
			Center[x].material = Colors[2];
			Border[x].material = Colors[1];
			Alphabet[x].text = ShuffledAlphabet[x];
			
		}
		UtilityImages[0].sprite = Sprites[2];
		UtilityImages[1].sprite = null;
		Basis.text = "10";
		Debug.LogFormat("[Alphabet Tiles #{0}] The timer has started", moduleId);
		while (Basis.text != "0")
		{
			Audio.PlaySoundAtTransform(SFX[8].name, transform);
			yield return new WaitForSecondsRealtime(1f);
			Basis.text = (Int32.Parse(Basis.text) - 1).ToString();
		}
		Debug.LogFormat("[Alphabet Tiles #{0}] The time is over. No answer chosen. A strike is going to be given.", moduleId);
		StartCoroutine(AMistake());
	}
	
	IEnumerator AMistake()
	{
		PlayTime = false;
		for (int x = 0; x < Alphabet.Length; x++)
		{
			Alphabet[x].text = "";
		}
		UtilityImages[0].sprite = null;
		Audio2.clip = SFX[7];
		Audio2.Play();
		while (Audio2.isPlaying)
		{
			for (int a = 0; a < 2; a++)
			{
				for (int b = 0; b < Center.Length; b++)
				{
					Center[b].material = a == 0 ? Colors[3] : Colors[1];
					Border[b].material = a == 0 ? Colors[3] : Colors[1];
				}
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		Module.HandleStrike();
		Reset();
		GenerateASequence();
	}
	
	void Reset()
	{
		HL2.SetActive(true);
		for (int b = 0; b < Center.Length; b++)
		{
			Center[b].material = Colors[2];
			Border[b].material = Colors[1];
		}
		UtilityImages[0].sprite = Sprites[0];
		Loading.transform.localEulerAngles = new Vector3(90, 0, 0);
	}
	
	IEnumerator Winner()
	{
		PlayTime = false;
		for (int x = 0; x < Alphabet.Length; x++)
		{
			Alphabet[x].text = "";
		}
		UtilityImages[0].sprite = null;
		Audio2.clip = SFX[6];
		Audio2.Play();
		while (Audio2.isPlaying)
		{
			for (int a = 0; a < 2; a++)
			{
				for (int b = 0; b < Center.Length; b++)
				{
					Center[b].material = a == 0 ? Colors[4] : Colors[0];
					Border[b].material = a == 0 ? Colors[4] : Colors[0];
				}
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		Module.HandlePass();
		for (int c = 0; c < Center.Length; c++)
		{
			Center[c].material =  Colors[4];
			Border[c].material =  Colors[4];
		}
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To start the cycle of the module, use !{0} play | To toggle the sound in the cycle, use !{0} mute/unmute | To stop the cycle of the module, use !{0} stop | To select a letter during the countdown, use !{0} press [VALID LETTER FROM THE GRID] or !{0} press [A-E][1-5] to press the designated coordinate of the tile (letters are columns and numbers are rows)";
    #pragma warning restore 414
	
	bool PlayTime = false;
	string[] CoordinatesL = {"A", "B", "C", "D", "E"};
	string[] CoordinatesN = {"1", "2", "3", "4", "5"};
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(command, @"^\s*play\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UtilityImages[0].sprite != Sprites[0])
			{
				yield return "sendtochaterror You are not able to press play at this moment. Command ignored.";
				yield break;
			}
			UtilityButton.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*mute\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UtilityImages[1].sprite != Sprites[3])
			{
				yield return "sendtochaterror You are not able to mute the module since its already muted. Command ignored.";
				yield break;
			}
			MusicButton.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*unmute\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UtilityImages[1].sprite != Sprites[4])
			{
				yield return "sendtochaterror You are not able to unmute the module since its already unmuted. Command ignored.";
				yield break;
			}
			MusicButton.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*stop\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (UtilityImages[0].sprite != Sprites[1])
			{
				yield return "sendtochaterror You are not able to press stop at this moment. Command ignored.";
				yield break;
			}
			UtilityButton.OnInteract();
		}
		
		if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) && parameters.Length > 1)
		{
			yield return null;
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Parameter length invalid. Command ignored.";
				yield break;
			}
			
			if (PlayTime == false)
			{
				yield return "sendtochaterror You are not able to press these buttons at this moment. Command ignored.";
				yield break;
			}
			
			if (parameters[1].Length == 1)
			{
				if (!parameters[1].ToUpper().EqualsAny(RegularAlphabet) || parameters[1].ToUpper() == ShuffledAlphabet[25])
				{
					yield return "sendtochaterror Command being sent does not contain a valid single letter. Command ignored.";
					yield break;
				}
				
				int Press = Array.IndexOf(ShuffledAlphabet, parameters[1].ToUpper());
				
				yield return "strike";
				yield return "solve";
				MainButtons[Press].OnInteract();
			}
			
			else if (parameters[1].Length == 2)
			{
				if (!parameters[1][0].ToString().ToUpper().EqualsAny(CoordinatesL) || !parameters[1][1].ToString().EqualsAny(CoordinatesN))
				{
					yield return "sendtochaterror Command being sent is not a valid coordinate. Command ignored.";
					yield break;
				}
				
				yield return "strike";
				yield return "solve";
				MainButtons[(Array.IndexOf(CoordinatesN, parameters[1][1].ToString()) * 5) % 25 + Array.IndexOf(CoordinatesL, parameters[1][0].ToString().ToUpper())].OnInteract();
			}
		}
	}
}