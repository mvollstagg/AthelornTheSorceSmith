using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        // Check for a key press to toggle pause
        if (Input.GetKeyDown(KeyCode.F12)) // You can change the key if you want
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0f;
        Time.timeScale = 1f;

        if (isPaused)
        {
            // Pause the game
            Debug.Log("Game Paused");
        }
        else
        {
            // Resume the game
            Debug.Log("Game Resumed");
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log($"OnApplicationPause called with status: {pauseStatus}");
        isPaused = pauseStatus;

        if (isPaused)
        {
            // Additional actions when the game is paused
        }
        else
        {
            // Additional actions when the game resumes
        }
    }
}
