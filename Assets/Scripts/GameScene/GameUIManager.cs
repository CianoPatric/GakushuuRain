using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance {get; private set;}

    public GSRootBinder rootBinder;
    public GameObject pauseMenu;
    private bool isPaused = false;

    public GameObject notebook;
    private bool isNotebook = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Notebook();
        }
    }

    public void Pause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Notebook()
    {
        isNotebook = !isNotebook;
        if (isNotebook)
        {
            NotebookManager.instance.OpenNotebook();
        }
        else
        {
            NotebookManager.instance.CloseNotebook();
        }
    }

    public async void SaveGame()
    {
        await DataManager.Instance.SaveData();
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        rootBinder.HandleGoToMainMenuButtonClick();
    }
}