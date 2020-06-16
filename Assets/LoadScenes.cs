using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Single()
    {
        SceneManager.LoadScene("HangmanSingle");
    }

    public void Multi()
    {
        SceneManager.LoadScene("HangmanMulti");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
