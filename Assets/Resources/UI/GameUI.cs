using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Drawing;

public class GameUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] float remainingTime;
    [SerializeField] float score;
    public float RemainingTime;
    public float Score;
    public GameObject Help;

    // Update is called once per frame
    void FixedUpdate()
    {
        remainingTime = PlankManager.Instance.liveTime;
        score = PlankManager.Instance.killedEnemy * 10 + (int)Mathf.Floor(PlankManager.Instance.liveTime);
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        scoreText.text = string.Format("{0}", score);
        RemainingTime = remainingTime;
        Score = score;
    }

    public void ToggleHelp()
    { 
        Help.SetActive(!Help.activeSelf);
        if (Help.activeSelf)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
