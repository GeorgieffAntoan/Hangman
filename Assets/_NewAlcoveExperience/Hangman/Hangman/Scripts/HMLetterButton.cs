using UnityEngine;
using System;
using UnityEngine.UI;


public class HMLetterButton:MonoBehaviour{
    
    public Text UIText;							// Reference to the text element
    public Color wrongLetterColor = Color.red;					// Color applied if the wrong letter is selected
    
    public void Start() {
    	HMGameController.instance.allKeyboardLetters = HMGameController.instance.allKeyboardLetters + UIText.text;		// Add the letter of this button to the allKeyboardLetters string of the game controller
    }
    
    [PunRPC]
    public void Click() {
    	if(!HMGameController.instance.solved && !HMGameController.instance.failed){				// Check that player has not solved the word
    		string t = UIText.text.ToUpper();												// Get text from text element and convert it to upper case
    		if(!HMGameController.instance.CheckLetter(t))										// Check if the letter is in the current game word
    			{
                        var tmp_cs1 = GetComponent<Button>().colors;
                        tmp_cs1.disabledColor = wrongLetterColor;
                        GetComponent<Button>().colors = tmp_cs1;
                    }	// Change background color if wrong letter is selected		
    		GetComponent<Button>().interactable = false;										// Disable button so player can no longer choose this letter
    	}
    }

//Quick way to change names of buttons to their respective letter
//function OnDrawGizmos () {
//	var t:String = UIText.text.ToUpper();
//	gameObject.name = "Letter Button - " +t;
//}
}
