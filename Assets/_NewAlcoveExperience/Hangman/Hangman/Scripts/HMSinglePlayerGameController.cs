﻿using UnityEngine;
using System;
using UnityEngine.UI;
using System.Globalization;

public class HMSinglePlayerGameController : MonoBehaviour
{

    public Text UISolveText;					// Reference to UI text objects
    public Text UITopicText;
    public Text UISolutionText;
    public Button UINextButton;

    public GameObject[] hangmanStates;				// Manually assing states in the scene of the different states of the hanged man
    public GameObject hangmanWin;					// Win screen game object
    public HMTopics[] topics;                       // Manually assign topics with words to this array

    int currentHangmanState;		// The current index of hangmanState (game object on stage)
    string currentTopic;			// Current topic name, used for the topic text
    int currentTopicIndex;			// Topic index keeps track of what HMTopics is in use
    string[] words;
    string currentWord;				// What word is the player trying to solve
    int currentWordLength;			// How long is the word (not counting spaces)
    int lettersFound;               // How many letters has the player found, used to check if word is complete

    [HideInInspector]
    public bool solved;							// Has player solved the word
    [HideInInspector]
    public bool failed;							// Has player failed the word
    [HideInInspector]
    public string allKeyboardLetters;               // Contains a string with all letters that is used in the on-screen keyboard

    public static HMSinglePlayerGameController instance;		// HMGameController is a singleton. HMGameController.instance.DoSomeThing();
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    public void OnApplicationQuit()
    {
        instance = null;
    }

    public void Awake()
    {                           // Make sure there are no other instances of this gameobject
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    } 

    public void Start()
    {
        UINextButton.interactable = false;          // Disable the restart button
        UISolutionText.text = "";                   // Wipe out the solution text
        RandomTopic();                              // Find a random topic from the topic array
        words = topics[currentTopicIndex].words.Split(topics[currentTopicIndex].splitCharacter[0]);     // Split the words from the topics into a array of strings
        GetWord();                                  // Find a random word from the words array
        Invoke("LateStart", 0.01f);					// Delayed part of the start so that the keyboard script has finished adding letters to allKeyboardLetters variable
    }

    public void LateStart()
    {
        currentWordLength = CountLettersInWord();	// Count how many letters the word is using
    }

    public void Restart()
    {                                   // Very simple way to restart game
        Application.LoadLevel(Application.loadedLevel);
    }

    public bool CheckLetter(string l)
    {                   // Check if a letter is in the word
        bool foundLetter = false;
        string n = UISolveText.text;                        // Get the solve word text in the UI
        for (int i = 0; i < currentWord.Length; i++)
        {       // Loops to find multiple letters
            if (currentWord[i].ToString() == l)
            {                       // The letter has been found
                n = n.Remove(i, 1);                             // Removes the "-" (dash) from the solve text 
                n = n.Insert(i, l);                             // Replace it with the letter function is looking for
                foundLetter = true;                             // A letter has been found						
                lettersFound++;                                 // Increase the letterFound so that it is possible to figure out if all letters in the word has been found
            }
        }
        if (foundLetter)
        {                                       // Check if a letter has been found
            UISolveText.text = n;                               // Change the solve word text in the UI
            if (lettersFound >= currentWordLength)
            {               // Check if the complete word has been solved
                SolvedWord();                                   // The word has been solved
            }
        }
        else
        {
            WrongLetter();                                      // No letters has been solved
        }
        return foundLetter;										// Return true or false, used to change letter button colors (red if wrong)
    }

    public void SolvedWord()
    {                                       // The complete word has been solved
        solved = true;                                          // solved is true, now player can not press the letter buttons
        hangmanWin.SetActive(true);                             // Activates the win game object screen
        hangmanWin.transform.position = hangmanStates[currentHangmanState].transform.position;  // Position the win screen to the right position ( "Hangman State 0" gameobject)
        hangmanStates[currentHangmanState].SetActive(false);    // Disables the last hangman state object
        UINextButton.interactable = true;						// Player can now press the restart button
    }

    public void WrongLetter()
    {
        if (!failed)
        {
            if (currentHangmanState > hangmanStates.Length - 3)
            {           // Check if player has reached the end of the hangman state array
                failed = true;                                          // failed is true, now player can not press the letter buttons
                UINextButton.interactable = true;                       // Player can now press the restart button
                UISolutionText.text = currentWord;                      // Display the correct answer in the UI
            }
            hangmanStates[currentHangmanState].SetActive(false);        // Disable the current Hangman state object
            currentHangmanState++;
            hangmanStates[currentHangmanState].transform.position = hangmanStates[currentHangmanState - 1].transform.position;  // Position the next Hangman state object to the right position ( "Hangman State 0" gameobject)
            hangmanStates[currentHangmanState].SetActive(true);         // Enable the next Hangman state object
        }
    }

    public void GetWord()
    {
        currentWord = words[UnityEngine.Random.Range(0, words.Length)].ToUpper();		// Find a random word from the words array
    }

    public int CountLettersInWord()
    {                                       //Counts letters in the current word, only counts letters that is represented in the keyboard
        int c = currentWord.Length;
        int s = 0;
        UISolveText.text = "";
        for (int i = 0; i < currentWord.Length; i++)
        {
            if (!CheckLetterKeyboard(currentWord[i].ToString()))
            {
                s++;
                UISolveText.text = UISolveText.text + currentWord[i].ToString();    // Set text that is not represented by the keyboard, like numbers
            }
            else
            {
                UISolveText.text = UISolveText.text + "-";                          // Set dashes in solve text like ---- --- ----
            }
        }  
        return c - s;															//Return letter count without spaces
    }

    public bool CheckLetterKeyboard(string s)
    {                           // Check if a letter is represented in the keyboard
        for (int i = 0; i < allKeyboardLetters.Length; i++)
        {
            if (s == allKeyboardLetters[i].ToString())
            {
                return true;                                                // The letter has been found in the keyboard, this letter will be hidden with a dash in the solve text
            }
        }
        return false;														// The letter has not been found in the keyboard, this letter will be visible in the solve text when it appears
    }

    public void RandomTopic()
    {
        currentTopicIndex = UnityEngine.Random.Range(0, topics.Length);                 // Find a random topic from the topic array
        currentTopic = topics[currentTopicIndex].topic.ToUpper();           // Set current topic string
        UITopicText.text = currentTopic;                                    // Change UI topic text
        UITopicText.color = topics[currentTopicIndex].topicColor;			// Change color of topic text
    }
}
