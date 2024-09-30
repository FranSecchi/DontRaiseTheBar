using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KnifeCakeMinigame : MonoBehaviour
{

    [SerializeField]private Transform center;
    [SerializeField]private float speed;
    [SerializeField]private float totalAngle;
    private float targetAngle;
    
    private float firstAngle = 0;
    private float secondAngle = 0;
    private float currentAngle = 0;
    private float score;

    [SerializeField] private GameObject panel;

    private Animator animator;

    private bool makingAnimation;
    private bool ended;

    [SerializeField]private TextMeshProUGUI scoreText;
    
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ended = false;
        makingAnimation = false;
        targetAngle = totalAngle / 4;
    }

    private void Update()
    {
        if (!makingAnimation && !ended)
        {
            transform.RotateAround(center.position, Vector3.forward, speed * Time.deltaTime);
            currentAngle += speed * Time.deltaTime;
            


            if (Input.GetMouseButtonDown(0))
            {
                if (firstAngle == 0)
                {
                    firstAngle = currentAngle % 360; // Ensure angle is between 0 and 360
                    
                    makingAnimation = true;
                    animator.Play("Gavinet");
                    StartCoroutine(WaitForTwoSeconds(0.5f));
                }
                else if (secondAngle == 0)
                {
                    secondAngle = currentAngle % 360; // Ensure angle is between 0 and 360
                    
                    animator.Play("Gavinet");
                    ended = true;
                    
                    // Calculate the angle between the two cuts
                    float angleDifference = Mathf.Abs(secondAngle - firstAngle);
                    if (angleDifference > 180)
                    {
                        angleDifference = 360 - angleDifference;
                    }

                    // Calculate how close the angle difference is to 90 degrees
                    float difference = Mathf.Abs(targetAngle - angleDifference);
                    float percentage = Mathf.Max(0f, 100f - (difference / targetAngle) * 100f);
                    score = GetWeightedValue(percentage);
                    // Update the score text
                    StartCoroutine(ShowTextAndReturn(2f));
                }
            }
        }

        

        
    }
    IEnumerator WaitForTwoSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        makingAnimation = false;
    }
    
    IEnumerator ShowTextAndReturn(float duration)
    {
        yield return new WaitForSeconds(duration);
        panel.SetActive(true);
        scoreText.text = "+"+Mathf.FloorToInt(score).ToString()+"!!";
        if (score < 0)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.fail, new Vector3(0,0,0));
            GameManager.instance.lifes--;
            GameManager.instance.lastNpcHappy = false;
        }
        else
        {
            GameManager.instance.score += Mathf.FloorToInt(score);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.sucess, new Vector3(0,0,0));
            GameManager.instance.lastNpcHappy = true;
        }
    }

    void playCutSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.knife_cut, new Vector3(0,0,0));
    }

    public void originalScene()
    {
        GameManager.instance.loadDayScene();
    }
    
    int GetWeightedValue(float inputValue)
    {
        if (inputValue < 50f)
            return 0;

        // Normalize the inputValue to a range of 0 to 1 between 50 and 100
        float normalizedValue = (inputValue - 50f) / 50f;

        // Apply a weighting function (square to give more weight to values closer to 100)
        float weightedValue = normalizedValue * normalizedValue;

        // Scale to the desired range of 0 to 15
        int finalValue = Mathf.RoundToInt(weightedValue * 100f);

        return finalValue;
    }
    
}
