using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    public GameObject notebook;
    private bool isNotebook = false;
    
    private NotebookManager _notebookManager;
    private DataManager _dataManager;
    private GSRootBinder _rootBinder;

    public void Initialize(NotebookManager notebookManager, DataManager dataManager, GSRootBinder rootBinder)
    {
        _notebookManager = notebookManager;
        _dataManager = dataManager;
        _rootBinder = rootBinder;
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
            _notebookManager.OpenNotebook();
        }
        else
        {
            _notebookManager.CloseNotebook();
        }
    }

    public async void SaveGame()
    {
        await _dataManager.SaveData();
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        _rootBinder.HandleGoToMainMenuButtonClick();
    }
}