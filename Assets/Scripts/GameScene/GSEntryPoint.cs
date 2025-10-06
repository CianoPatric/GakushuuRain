using R3;
using UnityEngine;

public class GSEntryPoint: MonoBehaviour
{
    [SerializeField] private GSRootBinder sceneUIRootPrefab;
    // ReSharper disable Unity.PerformanceAnalysis
    public Observable<GSExitParams> Run(DIContainer container, GSEnterParams gameSceneEnterParams)
    {
        Time.timeScale = 1f;
        GSRegistrationDI.Register(container, gameSceneEnterParams);
        var gameSceneViewModelContainer = new DIContainer(container);
        GSViewDIRegistration.Register(gameSceneViewModelContainer);
        
        var dataManager = container.Resolve<DataManager>();
        var wordLibraryManager = container.Resolve<WordLibraryManager>();
        var dialogueLibraryManager = container.Resolve<DialogueLibraryManager>();
        var cosmeticsLibraryManager = container.Resolve<CosmeticsLibraryManager>();
        
        var sceneInstance = Instantiate(sceneUIRootPrefab);
        var uiRoot = container.Resolve<LoadingScreenRootView>();
        uiRoot.AttachSceneUI(sceneInstance.gameObject);
        
        var player = sceneInstance.GetComponentInChildren<PlayerMovement>();
        var notebookView = sceneInstance.GetComponentInChildren<NotebookView>();
        var customizationView = sceneInstance.GetComponentInChildren<CustomizationView>();
        var dialogueManager = sceneInstance.GetComponentInChildren<DialogueManager>();
        var gameUIManager = sceneInstance.GetComponent<GameUIManager>();
        var NPCs = sceneInstance.GetComponentsInChildren<NpcLogic>();
        var dialogueCliclHandler = sceneInstance.GetComponentInChildren<DialogueTextClickHandler>();
        
        if (player == null || notebookView == null || customizationView == null || dialogueManager == null || gameUIManager == null || dialogueCliclHandler == null)
        {
            Debug.LogError("Один из ключевых компонентов на игровой сцене не найдены!");
            return Observable.Empty<GSExitParams>();
        }
        
        dataManager.InitializeWithData(gameSceneEnterParams.InitialPlayerData);
        
        player.Initialize(cosmeticsLibraryManager);
        dialogueManager.Initialize(dataManager, dialogueLibraryManager, wordLibraryManager, notebookView);
        notebookView.Initialize(dataManager, wordLibraryManager);
        customizationView.Initialize(dataManager, cosmeticsLibraryManager, player);
        gameUIManager.Initialize(notebookView, dataManager, sceneInstance, customizationView);
        dialogueCliclHandler.Initialize(dialogueManager);

        foreach (var npc in NPCs)
        {
            npc.Initialize(dataManager, dialogueManager);
        }
        
        if (gameSceneEnterParams?.InitialPlayerData != null)
        {
            var playerData = gameSceneEnterParams.InitialPlayerData;
            if (playerData.state != null)
            {
                player.transform.position = new Vector3(playerData.state.posX, playerData.state.posY, 0);
            }
            player.UpdateAppearance(playerData.profile.equippedItems);
            notebookView.PopulateFromSave();
        }
        else
        {
            Debug.LogWarning("Сцена была запущена без авторизации. Используется режим отладки");
        }
        
        var exitSceneSignalSubj = new Subject<Unit>();
        sceneInstance.Bind(exitSceneSignalSubj);
        
        var exitToUISceneSignal = exitSceneSignalSubj.Select(_ =>
        {
            var enterParams = new MMEnterParams("Returned from game");
            return new GSExitParams(enterParams);
        });
        return exitToUISceneSignal;
    }
}