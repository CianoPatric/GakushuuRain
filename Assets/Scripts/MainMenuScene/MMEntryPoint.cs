using R3;
using UnityEngine;

public class MMEntryPoint: MonoBehaviour
{
    [SerializeField] private MMRootBinder _sceneUIRootPrefab;
    
    public Observable<MMExitParams> Run(DIContainer container, MMEnterParams uiEnterParams)
    { 
        MMRegistrationDI.Register(container, uiEnterParams); 
        var uiViewModelContainer = new DIContainer(container); 
        MMViewDIRegistration.Register(uiViewModelContainer);
        
        var uiScene = Instantiate(_sceneUIRootPrefab); 
        var uiRoot = container.Resolve<LoadingScreenRootView>(); 
        uiRoot.AttachSceneUI(uiScene.gameObject);
        
        var mainMenuUI = uiScene.GetComponentInChildren<MainMenuUI>();
        if (mainMenuUI == null)
        {
            Debug.LogError("Не удалось найти MainMenuUI");
        }
        
        var exitSceneSignalSubj = new Subject<Unit>();
        uiScene.Bind(exitSceneSignalSubj);
        var exitToUISceneSignal = exitSceneSignalSubj.Select(_ =>
        {
            PlayerData authenticatedPlayerData = mainMenuUI.GetAuthenticatedPlayerData();
            if (authenticatedPlayerData == null)
            {
                Debug.LogError("Игрок не авторизован");
                
                return null;
            }
            
            var enterParams = new GSEnterParams(authenticatedPlayerData);
            var exitParams = new MMExitParams(enterParams);
            return exitParams;
        }).WhereNotNull();
        return exitToUISceneSignal;
    }
}