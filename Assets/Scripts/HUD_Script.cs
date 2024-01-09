using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD_Script : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] TMP_Text score_text;
    [SerializeField] TMP_Text info_text;
    [SerializeField] GameObject game_overlay;
    [SerializeField] GameObject background_image;
    bool is_paused;
    bool set_active;
    static public int score_counter;

    void Start()
    {
        score_counter = 0;
        is_paused = false;
    }

    void Update()
    {
        score_text.SetText("Score: " + score_counter);

        ShowInfoScreen();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            info_text.gameObject.SetActive(false);
        }
    }

    void ShowInfoScreen()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !is_paused)
            PauseGame(0);
        else if (Input.GetKeyDown(KeyCode.Escape) && is_paused)
            PauseGame(1);
    }

    private void PauseGame(float time_scale)
    {
        if (time_scale == 0)
        {
            is_paused = true;
            set_active = true;
        }
        else
        {
            is_paused = false;
            set_active = false;
        }
        Time.timeScale = time_scale;
        background_image.SetActive(set_active);
        game_overlay.SetActive(!set_active);
        Player_Controller_Script.allow_controls = !set_active;
    }

    public void PauseButton(bool toggle)
    {
        is_paused = toggle;
        if (toggle == true)
            PauseGame(0);
        else
            PauseGame(1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
        PauseGame(1);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        PauseGame(1);
    }
}
