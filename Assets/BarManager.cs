using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BarManager : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager _gameManager;

    [Header("Characters")] 
    
    [SerializeField] private Sprite[] cat;
    [SerializeField] private Sprite[] snake;
    [SerializeField] private Sprite[] fox;
    [SerializeField] private Sprite[] mcChickenNugget;
    [SerializeField] private Sprite[] ape;
    [SerializeField] private Sprite[] racoon;

    private List<Sprite[]> dayNpc;
    private List<Sprite[]> nightNpc;



    [Header("Things ")] [SerializeField] private Image panelImage;

    [SerializeField] private Image life1;
    [SerializeField] private Image life2;
    [SerializeField] private Image life3;
    
    [SerializeField] private Image client;

    [SerializeField] private Image fons;

    [SerializeField] private Animator animatorClient;

    [SerializeField] private TextMeshProUGUI score;

    [SerializeField] private int minigamePerTime;

    [SerializeField] private Sprite day, night;


    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        
        dayNpc = new List<Sprite[]>();

        if (_gameManager.lifes == 2)
        {
            life3.enabled = false;
        }
        else if (_gameManager.lifes == 1)
        {
            life3.enabled = false;
            life2.enabled = false;
        }
        else if (_gameManager.lifes == 0)
        {
            life3.enabled = false;
            life2.enabled = false;
            life1.enabled = false;
        }

        dayNpc.Add(mcChickenNugget);
        dayNpc.Add(ape);
        dayNpc.Add(racoon);
        dayNpc.Add(cat);
        dayNpc.Add(snake);
        dayNpc.Add(fox);
        


        score.text = _gameManager.score.ToString();
        
        
        if (_gameManager.walkIn)
        {
            SelectNpc();
            client.sprite = getNPCSprite();
            animatorClient.Play("walkIn");
            _gameManager.walkIn = false;
        }
        else
        {
            client.sprite = getNPCSprite();
            animatorClient.Play("walkOut");
            if (_gameManager.lastNpcHappy)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.coin_bonus, new Vector3(0,0,0));
            }
            _gameManager.walkIn = true;
        }

        StartCoroutine(FadeImageToWhite(1f));
        


    }

    private void fakeStart()
    {
        StartCoroutine(WaitForTwoSeconds());
    }


    private void SelectNpc()
    {
        int a = _gameManager.lastNPC;
        while (a == _gameManager.lastNPC)
        {
            a = Random.Range(0, 6);
        }

        _gameManager.lastNPC = a;

    }

    private Sprite getNPCSprite()
    {
        if (_gameManager.isDay)
        {
            if (_gameManager.walkIn)
            {
                return dayNpc[_gameManager.lastNPC][0];
            }
            else
            {
                if (_gameManager.lastNpcHappy)
                {
                    return dayNpc[_gameManager.lastNPC][1];
                }
                else
                {
                    return dayNpc[_gameManager.lastNPC][2];
                }
            }
        }
        else
        {
            if (_gameManager.walkIn)
            {
                return nightNpc[_gameManager.lastNPC][0];
            }
            else
            {
                if (_gameManager.lastNpcHappy)
                {
                    return nightNpc[_gameManager.lastNPC][1];
                }
                else
                {
                    return nightNpc[_gameManager.lastNPC][2];
                }
            }
        }
    }

    IEnumerator WaitForTwoSeconds()
    {
        yield return new WaitForSeconds(2f);

        StartCoroutine(FadeImageToBlack(1f));
    }

    IEnumerator FadeImageToBlack(float duration)
    {
        panelImage.color = new Color(0, 0, 0, 0);
        Color startColor = new Color(0, 0, 0, 0); // Transparent black
        Color endColor = Color.black; // Solid black
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panelImage.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }

        // Ensure the panel is completely black after the duration
        panelImage.color = endColor;
        loadGame();
    }
    
    IEnumerator FadeImageToWhite(float duration)
    {
        panelImage.color = Color.black;
        Color startColor = Color.black; // Solid black
        Color endColor = new Color(0, 0, 0, 0); // Transparent black
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panelImage.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }

        // Ensure the panel is completely transparent after the duration
        panelImage.color = endColor;
        fakeStart();
    }
    
    
    
    

    private void loadGame()
    {
        if (_gameManager.lifes ==0)
        {
            //you lose
            AudioManager.instance.PlayOneShot(FMODEvents.instance.glass, new Vector3(0,0,0));
            _gameManager.lose();
                
        }
        else
        {
        if (_gameManager.walkIn)
        {
            _gameManager.loadDayScene();
        }
        else
        {
            /*
            if (_gameManager.lastMinigame == 0)
            {
                _gameManager.loadDrinkMinigame();
                _gameManager.lastMinigame = 1;

            }
            else if (_gameManager.lastMinigame == 1)
            {
                _gameManager.loadCutMinigame();
                
                _gameManager.lastMinigame = 0;
            }*/

            int i = Random.Range(0, 3);

            if (i == 2 && _gameManager.lastMinigame != 2)
            {
                _gameManager.loadCutMinigame();
                _gameManager.lastMinigame = 2;
            }
            else
            {
                _gameManager.loadDrinkMinigame();
                _gameManager.lastMinigame = 0;
                
            }
            


        }
        }
    }
}



