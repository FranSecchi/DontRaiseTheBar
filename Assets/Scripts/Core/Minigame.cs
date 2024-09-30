using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    public int scene;
    public GameObject[] minigames;
    public GameObject menu;
    public Button reset;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI finalText;
    private float time = 0f;
    private int count = 0;
    private int x = 0;
    private void Start()
    {
        x = Random.Range(0, minigames.Length);
        minigames[x].SetActive(true);
    }
    public void WriteTime()
    {
        time += Time.deltaTime;
        timeText.text = time.ToString("F2") + " s";
    }
    public void WriteText(string txt)
    {
        if(!menu.activeSelf) menu.SetActive(true);
        finalText.text = txt;
    }
    public void ResetMinigame()
    {
        SceneManager.LoadScene(scene);
    }
}
