using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI PlankText;
    public void Setup(int score, string time, int plank) {
        gameObject.SetActive(true);
        ScoreText.text = "Score: " + score;
        TimeText.text = "Lived Time: " + time;
        PlankText.text = "Max Plank Num: " + plank;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
