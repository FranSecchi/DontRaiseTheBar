using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager _gameManager;

    [SerializeField]private TextMeshProUGUI highscore;
    
    
    void Start()
    {
       StartCoroutine(wait1sec());
        

    }

    IEnumerator wait1sec()
    {
        yield return new WaitForSeconds(0.2f);
        _gameManager = FindAnyObjectByType<GameManager>();

        highscore.text = _gameManager.highscore.ToString();
    }
    
    public void StartGame()
    {
        _gameManager.StartGame();
    }
    

    public void Quit()
    {
        _gameManager.Exit();
    }
}
