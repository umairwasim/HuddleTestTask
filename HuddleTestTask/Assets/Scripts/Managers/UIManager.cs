using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameplayMenu;
    [SerializeField] private GameObject gameWonMenu;
    [SerializeField] private GameObject gameLostMenu;

    [Header("Buttons")]
    [SerializeField] private Button playButton;

    public void PlayButtonPressed()
    {
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
