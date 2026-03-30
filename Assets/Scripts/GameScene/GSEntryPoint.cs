using R3;
using UnityEngine;

public class GSEntryPoint: MonoBehaviour
{
    [SerializeField] private GSRootBinder sceneUIRootPrefab;
    
    private readonly CompositeDisposable _disposables = new();
    // ReSharper disable Unity.PerformanceAnalysis
    public Observable<GSExitParams> Run(DIContainer container, GSEnterParams gameSceneEnterParams, GameEntryPoint gameEntryPoint)
    {
        Time.timeScale = 1f;
        GSRegistrationDI.Register(container, gameSceneEnterParams);
        var gameSceneViewModelContainer = new DIContainer(container);
        GSViewDIRegistration.Register(gameSceneViewModelContainer);

        var sceneInstance = Instantiate(sceneUIRootPrefab);
        var uiRoot = container.Resolve<LoadingScreenRootView>();
        uiRoot.AttachSceneUI(sceneInstance.gameObject);
        
        var dataManager = container.Resolve<DataManager>();
        var wordLibraryManager = container.Resolve<WordLibraryManager>();
        var dialogueLibraryManager = container.Resolve<DialogueLibraryManager>();
        var cosmeticsLibraryManager = container.Resolve<CosmeticsLibraryManager>();
        
        var player = sceneInstance.GetComponentInChildren<PlayerMovement>();
        var notebookView = sceneInstance.GetComponentInChildren<NotebookView>();
        var playerAppearance = player.GetComponent<PlayerAppearance>();
        var customizationView = sceneInstance.GetComponentInChildren<CustomizationView>();
        var dialogueManager = sceneInstance.GetComponentInChildren<DialogueManager>();
        var gameUIManager = sceneInstance.GetComponent<GameUIManager>();
        var dialogueClickHandler = sceneInstance.GetComponentInChildren<DialogueTextClickHandler>();
        
        var customizationService = new CustomizationService(dataManager, cosmeticsLibraryManager);
        container.RegisterInstance(customizationService);
        
        if (player == null || notebookView == null || customizationView == null || dialogueManager == null || gameUIManager == null || dialogueClickHandler == null)
        {
            Debug.LogError("Один из ключевых компонентов на игровой сцене не найдены!");
            return Observable.Empty<GSExitParams>();
        }

        var interactables = sceneInstance.GetComponentsInChildren<IInteractable>(true);
        foreach (var interactable in interactables)
        {
            if (interactable is Door door)
            {
                door.Initialize(gameEntryPoint);
            }
            else if (interactable is NpcLogic npc)
            {
                npc.Initialize(dataManager, dialogueManager);
            }
        }
        
        dataManager.InitializeWithData(gameSceneEnterParams.InitialPlayerData);
        player.Initialize(cosmeticsLibraryManager, playerAppearance);
        dialogueManager.Initialize(dataManager, dialogueLibraryManager, wordLibraryManager, notebookView, playerAppearance);
        notebookView.Initialize(dataManager, wordLibraryManager, player);
        customizationService.BindPlayer(playerAppearance);
        customizationView.Initialize(customizationService, cosmeticsLibraryManager);
        gameUIManager.Initialize(notebookView, dataManager, sceneInstance, customizationView);
        dialogueClickHandler.Initialize(dialogueManager);
        
        if (gameSceneEnterParams?.InitialPlayerData != null)
        {
            var playerData = gameSceneEnterParams.InitialPlayerData;
            if (playerData.state != null)
            {
                player.transform.position = new Vector3(playerData.state.posX, playerData.state.posY, 0);
            }
            notebookView.PopulateFromSave();
        }
        else
        {
            Debug.LogWarning("Сцена была запущена без авторизации. Используется режим отладки");
        }
        
        var exitSceneSignalSubj = new Subject<Unit>();
        sceneInstance.Bind(exitSceneSignalSubj);
        
        return exitSceneSignalSubj.Select(_ =>
        {
            _disposables.Dispose();
            return new GSExitParams(new MMEnterParams("Returned from game"));
        });
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}