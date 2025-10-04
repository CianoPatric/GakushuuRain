using R3;
using UnityEngine;

public class GSEntryPoint: MonoBehaviour
{
    [SerializeField] private GSRootBinder _sceneUIRootPrefab;
    private GameObject avatar;
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
        
        var sceneInstance = Instantiate(_sceneUIRootPrefab);
        var uiRoot = container.Resolve<LoadingScreenRootView>();
        uiRoot.AttachSceneUI(sceneInstance.gameObject);
        
        var player = sceneInstance.GetComponentInChildren<PlayerMovement>();
        var notebookManager = sceneInstance.GetComponentInChildren<NotebookManager>();
        var dialogueManager = sceneInstance.GetComponentInChildren<DialogueManager>();
        var gameUIManager = sceneInstance.GetComponent<GameUIManager>();
        var NPCs = sceneInstance.GetComponentsInChildren<NpcLogic>();
        var dialogueCliclHandler = sceneInstance.GetComponentInChildren<DialogueTextClickHandler>();
        
        if (player == null || notebookManager == null || dialogueManager == null || gameUIManager == null || dialogueCliclHandler == null)
        {
            Debug.LogError("Один из ключевых компонентов на игровой сцене не найдены!");
            return Observable.Empty<GSExitParams>();
        }
        
        dataManager.InitializeWithData(gameSceneEnterParams.InitialPlayerData);
        
        player.Initialize(cosmeticsLibraryManager);
        dialogueManager.Initialize(dataManager, dialogueLibraryManager, wordLibraryManager, notebookManager);
        notebookManager.Initialize(dataManager, wordLibraryManager, cosmeticsLibraryManager, player);
        gameUIManager.Initialize(notebookManager, dataManager, sceneInstance);
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
            notebookManager.PopulateFromSave();
        }
        else
        {
            Debug.LogWarning("Сцена была запущена без авторизации. Используется режим отладки");
        }

        dialogueManager.HideDialogueLine();
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