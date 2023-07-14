using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
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

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI moveText;
    private int moveCounter = 0;

    private void Start()
    {

        ActivateMenu(menu);
        GameManager.Instance.OnGameWon += GameManager_OnGameWon;
        GameManager.Instance.OnGameLost += GameManager_OnGameLost;
        GameManager.Instance.OnMoveCounter += GameManager_OnMoveCounter;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWon -= GameManager_OnGameWon;
        GameManager.Instance.OnGameLost -= GameManager_OnGameLost;
        GameManager.Instance.OnMoveCounter -= GameManager_OnMoveCounter;
    }

    private void GameManager_OnMoveCounter()
    {
        moveCounter++;
        moveText.text = "MOVES: " + moveCounter;
    }

    private void GameManager_OnGameLost()
    {
        menu = MenuState.Lost;
        ActivateMenu(menu);
    }

    private void GameManager_OnGameWon()
    {
        menu = MenuState.Won;
        ActivateMenu(menu);
    }

    private void ActivateMenu(MenuState menuToActivate)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
        }
        //Enable the selected menu only
        menus[(int)menuToActivate].SetActive(true);
    }

    public void PlayButtonPressed()
    {
        menu = MenuState.Game;
        ActivateMenu(menu);
        GameManager.Instance.ChangeGameState(GameState.GenerateGrid);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
