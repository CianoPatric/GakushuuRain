using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool _isPaused = false;

    public GameObject notebook;
    private bool _isNotebook = false;
    
    private NotebookView _notebookView;
    private CustomizationView _customizationView;
    private DataManager _dataManager;
    private GSRootBinder _rootBinder;

    public void Initialize(NotebookView notebookView, DataManager dataManager, GSRootBinder rootBinder, CustomizationView customizationView)
    {
        _customizationView = customizationView;
        _notebookView = notebookView;
        _dataManager = dataManager;
        _rootBinder = rootBinder;
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
        _isPaused = !_isPaused;
        pauseMenu.SetActive(_isPaused);
        if (_isPaused)
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
        _isNotebook = !_isNotebook;
        if (_isNotebook)
        {
            _notebookView.OpenNotebook();
        }
        else
        {
            _notebookView.CloseNotebook();
            _customizationView.CloseCustomizationPanel();
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