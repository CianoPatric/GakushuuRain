using UnityEngine;

public class LoadingScreenRootView: MonoBehaviour
{ 
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Transform _currentSceneContainer;
    private void Awake()
    { 
        HideLoadingScreen();
    }
    public void ShowLoadingScreen()
    {
        _loadingScreen.SetActive(true);
    }
    public void HideLoadingScreen()
    { 
        _loadingScreen.SetActive(false);
    }
    public void AttachSceneUI(GameObject sceneUI)
    { 
        ClearSceneUI(); 
        sceneUI.transform.SetParent(_currentSceneContainer, false);
    }
    private void ClearSceneUI()
    { 
        var childCount = _currentSceneContainer.childCount; 
        for(int i = 0; i < childCount; i++)
        { 
            Destroy( _currentSceneContainer.GetChild(i).gameObject);
        }
    }
}