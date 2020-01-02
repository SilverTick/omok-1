using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BoardSelector : MonoBehaviour
{
    public characterSelections c; // just for example
    public static BoardSelector Instance;

    // only called before Start()
    // have other scripts call BoardSelector data
    private void Awake()
    {
        /* Instance will 
         * not be destroyed when going into next Scene
         * destroyed when going back to previous Scene 
         */
        // if a board was never selected, instantiate it
        if (Instance == null) {
            Instance = this;
        }
        // board was instantiated, but player wants a different one
        else {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Change_Scene(string selection)
    {
        Debug.Log("change scene!");
        Debug.Log(selection);

        // keep board type selection in memory forever until restart
        PlayerPrefs.SetString("selection", selection);

        // can either do 1 or "Scenes/GameBoard", accepts int or string
        SceneManager.LoadScene("Scenes/GameBoard");
    }

}

// not a function, is an enumerated type
public enum characterSelections
{
    red,
    blue,
    green
}