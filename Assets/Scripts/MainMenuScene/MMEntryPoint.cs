using R3;
using UnityEngine;

public class MMEntryPoint: MonoBehaviour
{
    [SerializeField] private MMRootBinder _sceneUIRootPrefab;

    private Subject<MMExitParams> _exitSceneSignalSubj = new();

    public Observable<MMExitParams> Run(DIContainer container, MMEnterParams uiEnterParams)
    { 
        MMRegistrationDI.Register(container, uiEnterParams); 
        var uiViewModelContainer = new DIContainer(container); 
        MMViewDIRegistration.Register(uiViewModelContainer);
        
        var uiScene = Instantiate(_sceneUIRootPrefab); 
        var uiRoot = container.Resolve<LoadingScreenRootView>(); 
        uiRoot.AttachSceneUI(uiScene.gameObject);
        
        var exitSceneSignalSubj = new Subject<Unit>();
        uiScene.Bind(exitSceneSignalSubj);
        var enterParams = new GSEnterParams("DoublePenetration"); //надо вводить данные
        var exitParams = new MMExitParams(enterParams);
        var exitToUISceneSignal = exitSceneSignalSubj.Select(_ => exitParams);
        return exitToUISceneSignal;
    }
}