using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    public static GameManager instance = null;

    public int highscore;

    public int score;
    public int lifes;


    public int lastMinigame;
    public int minigameCount;
    
    public int lastNPC;
    public bool lastNpcHappy;

    public bool isDay;

    public bool walkIn;
    
    void Awake()
    {
       
        if (instance == null)
        {
            instance = this;
            highscore = 0;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            
        }
    }
    
    void Start()
    {
        
    }

    public void StartGame()
    {
        lifes = 3;
        score = 0;
        lastNPC = 0;
        lastMinigame = 0;
        isDay = true;
        walkIn = true;
        SceneManager.LoadScene(1);
    }

    public void loadDayScene()
    {
        SceneManager.LoadScene(1);
    }
    
    public void lose()
    {
        if (score > highscore)
        {
            highscore = score;
        }
        SceneManager.LoadScene(0);
    }


    public void loadDrinkMinigame()
    {
        SceneManager.LoadScene(2);
    }
    
    public void loadCutMinigame()
    {
        SceneManager.LoadScene(3);
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
