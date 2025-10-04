using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Scene;

public class GameEntryPoint 
{ 
    private static GameEntryPoint _instance;
    private Coroutines _coroutines;
    private LoadingScreenRootView _menuRoot;
    private readonly DIContainer _rootContainer = new();
    private DIContainer CasheContainer;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void FirstLoad()
    {
        _instance = new GameEntryPoint();
        _instance.StartAPP();
    }

    private GameEntryPoint()
    {
        _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
        Object.DontDestroyOnLoad(_coroutines.gameObject);
        
        var authManager = new GameObject("[AUTH_MANAGER]").AddComponent<AuthManager>();
        Object.DontDestroyOnLoad(authManager.gameObject);
        _rootContainer.RegisterInstance(authManager);
        
        var dataManager = new GameObject("[DATA_MANAGER]").AddComponent<DataManager>();
        Object.DontDestroyOnLoad(dataManager.gameObject);
        _rootContainer.RegisterInstance(dataManager);
        
        var wordLibraryManager = new GameObject("[WORD_LIBRARY_MANAGER]").AddComponent<WordLibraryManager>();
        Object.DontDestroyOnLoad(wordLibraryManager.gameObject);
        _rootContainer.RegisterInstance(wordLibraryManager);

        var cosmeticsLibraryManager = new GameObject("[COSMETICS_LIBRARY_MANAGER]").AddComponent<CosmeticsLibraryManager>();
        Object.DontDestroyOnLoad(cosmeticsLibraryManager.gameObject);
        _rootContainer.RegisterInstance(cosmeticsLibraryManager);
        
        var dialogueLibraryManager = new GameObject("[DIALOGUE_LIBRARY_MANAGER]").AddComponent<DialogueLibraryManager>();
        Object.DontDestroyOnLoad(dialogueLibraryManager.gameObject);
        _rootContainer.RegisterInstance(dialogueLibraryManager);
        
        var prefabUIRoot = Resources.Load<LoadingScreenRootView>("UIRoot");
        _menuRoot = Object.Instantiate(prefabUIRoot);
        Object.DontDestroyOnLoad(_menuRoot.gameObject);
        _rootContainer.RegisterInstance(_menuRoot);
    }
    private void StartAPP()
    {
#if UNITY_EDITOR
        var sceneName = SceneManager.GetActiveScene().name;
        if(sceneName == MAINMENU)
        {
            _coroutines.StartCoroutine(LoadAndStartMainMenu());
            return;
        }
        if(sceneName == GAMESCENE)
        {
            GSEnterParams _gameEnter = new GSEnterParams(new PlayerData());
            _coroutines.StartCoroutine(LoadAndStartGameScene(_gameEnter));
            return;
        }
        if (sceneName != BOOT)
        {
            return;
        }
#endif
        _coroutines.StartCoroutine(LoadAndStartMainMenu());
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator LoadAndStartMainMenu(MMEnterParams enterParams = null)
    {
        _menuRoot.ShowLoadingScreen();
        CasheContainer?.Dispose();
        yield return LoadScene(BOOT);
        yield return LoadScene(MAINMENU);
        yield return new WaitForSeconds(0.5f);
        var sceneEntryPoint = Object.FindFirstObjectByType<MMEntryPoint>();
        var UIContainer = CasheContainer = new DIContainer(_rootContainer);
        sceneEntryPoint.Run(UIContainer, enterParams).Subscribe(mainMenuExitParams =>
        {
            _coroutines.StartCoroutine(LoadAndStartGameScene(mainMenuExitParams.GameSceneEnterParams));
        });
        _menuRoot.HideLoadingScreen();
    }
    private IEnumerator LoadAndStartGameScene(GSEnterParams gameSceneEnterParams)
    {
        _menuRoot.ShowLoadingScreen();
        CasheContainer?.Dispose();
        yield return LoadScene(BOOT);
        yield return LoadScene(GAMESCENE);
        yield return new WaitForSeconds(0.5f);
        var sceneEntryPoint = Object.FindFirstObjectByType<GSEntryPoint>();
        var gameSceneContainer = CasheContainer = new DIContainer(_rootContainer);
        sceneEntryPoint.Run(gameSceneContainer, gameSceneEnterParams).Subscribe(gameSceneExitParams =>
        {
            _coroutines.StartCoroutine(LoadAndStartMainMenu(gameSceneExitParams.MainMenuEnterParams));
        });
        _menuRoot.HideLoadingScreen();
    }
    private IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    } 
}