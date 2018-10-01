using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public enum GameMode
{
    idle,
    playing,
    levelEnd

}
public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S; // a private  Singleton

    [Header("Set in Inspector")]

    public Text uiLevel;
    public Text uiShots;
    public Text uiButton;

    public Vector3 castlePos;
    public GameObject[] castles;   // An array of the castles
   
     // The place to put castles

   [Header("Set Dynamiccally")]


    // fields set dynamically
    public int level;     // The current level
    public int levelMax;  // The number of levels
    public int shotsTaken;
    public GameObject castle;    // The current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Slingshot"; // FollowCam mode

    void Start()
    {
        S = this; // Define the Singleton

        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        // Get rid of the old castle if one exists
        if (castle != null)
        {
            Destroy(castle);
        }

        // Destroy old projectiles if they exist
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        // Instantiate the new castle
        castle = Instantiate<GameObject> (castles[level]);
        castle.transform.position = castlePos;
      
        shotsTaken = 0;

        // Reset the camera
        SwitchView("Both");
        ProjectileLine.S.Clear();

        // Reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    
    void UpdateGUI()
    {
        // Show the data in the GUITexts
        uiLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uiShots.text = "Shots Taken: " + shotsTaken;
    }
    void Update()

    {
        UpdateGUI();

        // Check for level end
        if (mode == GameMode.playing && Goal.goalMet)
        {
            // Change mode to stop checking for level end
            mode = GameMode.levelEnd;
            // Zoom out
            SwitchView("show Both");
            // Start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            level = 0;
        }
        StartLevel();
    }

    public void SwitchView(string eView="")
    {
        if (eView == "")
        {
            eView = uiButton.text;
        }

        switch (showing)
        {
            case "Show slingshot":
                FollowCam.POI = null;
                uiButton.text = "Show Castle";
                break;

                

            case "Show Castle":

                FollowCam.POI = S.castle;
                uiButton.text = "Show Both";
                break;
                

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uiButton.text = "Show Slingshot";
                break;

        }
    }

    

    // Static method that allows code anywhere to increment shotsTaken
    public static void ShotFired()
    {
        S.shotsTaken++;
    }

}

