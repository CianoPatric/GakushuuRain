using R3;
using UnityEngine;

public class GSEntryPoint: MonoBehaviour
{
    [SerializeField] private GSRootBinder _sceneUIRootPrefab;
    // ReSharper disable Unity.PerformanceAnalysis
    public Observable<GSExitParams> Run(DIContainer container, GSEnterParams gameSceneEnterParams)
    {
        Time.timeScale = 1f;
        GSRegistrationDI.Register(container, gameSceneEnterParams);
        var gameSceneViewModelContainer = new DIContainer(container);
        GSViewDIRegistration.Register(gameSceneViewModelContainer);
        
        var uiScene = Instantiate(_sceneUIRootPrefab);
        var uiRoot = container.Resolve<LoadingScreenRootView>();
        uiRoot.AttachSceneUI(uiScene.gameObject);

        if (gameSceneEnterParams != null && gameSceneEnterParams.InitialPlayerData != null)
        {
            DataManager.Instance.InitializeWithData(gameSceneEnterParams.InitialPlayerData);
            if (NotebookManager.instance != null)
            {
                NotebookManager.instance.Initialize();
            }
            else
            {
                Debug.Log("NotebookManager не найден");
            }
        }
        else
        {
            Debug.Log("Сцена была запущена без авторизации");
        }
        
        var exitSceneSignalSubj = new Subject<Unit>();
        uiScene.Bind(exitSceneSignalSubj);
        Debug.Log($"{gameSceneEnterParams.InitialPlayerData}");
        var enterParams = new MMEnterParams("Fatality");
        var exitParams = new GSExitParams(enterParams);
        var exitToUISceneSignal = exitSceneSignalSubj.Select(_ => exitParams);
        return exitToUISceneSignal;
    }
}