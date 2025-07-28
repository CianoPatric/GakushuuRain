using R3;
using UnityEngine;

public class GSEntryPoint: MonoBehaviour
{
    [SerializeField] private GSRootBinder _sceneUIRootPrefab;
    // ReSharper disable Unity.PerformanceAnalysis
    public Observable<GSExitParams> Run(DIContainer container, GSEnterParams gameSceneEnterParams)
    {
        GSRegistrationDI.Register(container, gameSceneEnterParams);
        var gameSceneViewModelContainer = new DIContainer(container);
        GSViewDIRegistration.Register(gameSceneViewModelContainer);
        
        var uiScene = Instantiate(_sceneUIRootPrefab);
        var uiRoot = container.Resolve<LoadingScreenRootView>();
        uiRoot.AttachSceneUI(uiScene.gameObject);
        
        var exitSceneSignalSubj = new Subject<Unit>();
        uiScene.Bind(exitSceneSignalSubj);
        Debug.Log($"{gameSceneEnterParams.TypeGame}");
        //var grid = Object.FindFirstObjectByType<BuildingsGrid>();
        //var cam = Object.FindFirstObjectByType<ControllCamera>();
        //(cam as IInjectable)?.Inject(container);
        //(grid as IInjectable)?.Inject(container);
        var enterParams = new MMEnterParams("Fatality");
        var exitParams = new GSExitParams(enterParams);
        var exitToUISceneSignal = exitSceneSignalSubj.Select(_ => exitParams);
        return exitToUISceneSignal;
    }
}