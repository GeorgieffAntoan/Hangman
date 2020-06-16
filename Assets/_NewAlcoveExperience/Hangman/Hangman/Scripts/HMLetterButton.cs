using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HMLetterButton:MonoBehaviour{
    
    public Text UIText;							// Reference to the text element
    public Color wrongLetterColor = Color.red;					// Color applied if the wrong letter is selected
    PhotonView photonView;
    public bool mp;

    public void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("HangmanSingle")) mp = false; else mp = true;
        if (mp) HMGameController.instance.allKeyboardLetters = HMGameController.instance.allKeyboardLetters + UIText.text;
        else HMSinglePlayerGameController.instance.allKeyboardLetters = HMSinglePlayerGameController.instance.allKeyboardLetters + UIText.text;	// Add the letter of this button to the allKeyboardLetters string of the game controller
    }

    public void Check()
    {
        
    }
    public void Update()
    {
        Debug.Log(mp);
    }
    public void ChangeBool()
    {
        mp = true;
    }
    public void Click() {
        if (mp)
        {
            if (!HMGameController.instance.solved && !HMGameController.instance.failed)
            {               // Check that player has not solved the word
                string t = UIText.text.ToUpper();                                               // Get text from text element and convert it to upper case
                if (!HMGameController.instance.CheckLetter(t))                                      // Check if the letter is in the current game word
                {
                    var tmp_cs1 = GetComponent<Button>().colors;
                    tmp_cs1.disabledColor = wrongLetterColor;
                    GetComponent<Button>().colors = tmp_cs1;
                }   // Change background color if wrong letter is selected		
                GetComponent<Button>().interactable = false;                                        // Disable button so player can no longer choose this letter
            }
        }
        else
        {
            if (!HMSinglePlayerGameController.instance.solved && !HMSinglePlayerGameController.instance.failed)
            {               // Check that player has not solved the word
                string t = UIText.text.ToUpper();                                               // Get text from text element and convert it to upper case
                if (!HMSinglePlayerGameController.instance.CheckLetter(t))                                      // Check if the letter is in the current game word
                {
                    var tmp_cs1 = GetComponent<Button>().colors;
                    tmp_cs1.disabledColor = wrongLetterColor;
                    GetComponent<Button>().colors = tmp_cs1;
                }   // Change background color if wrong letter is selected		
                GetComponent<Button>().interactable = false;                                        // Disable button so player can no longer choose this letter
            }
        } 
    }

//Quick way to change names of buttons to their respective letter
//function OnDrawGizmos () {
//	var t:String = UIText.text.ToUpper();
//	gameObject.name = "Letter Button - " +t;
//}
}
