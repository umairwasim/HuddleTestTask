using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum MenuState
{
    Main,
    Game,
    Won,
    Lost,
}

public class UIManager : MonoBehaviour
{
    public MenuState menu = MenuState.Main;

    [Header("Menus")]
    [SerializeField] private GameObject[] menus; // 0 = main, 1 = game, 2 = won, 3 = lost

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button wonButton;
    [SerializeField] private Button lostButton;

    private void Start()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
        }
        //Enable the main menu only
        menus[(int)MenuState.Main].SetActive(true);
    }

    public void PlayButtonPressed()
    {
        menus[(int)MenuState.Main].SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.GenerateGrid);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
