using R3;
using UnityEngine;

public class GSRootBinder: MonoBehaviour
{
    private Subject<Unit> _exitSceneSignalSubj;
    public void HandleGoToMainMenuButtonClick()
    {
        _exitSceneSignalSubj?.OnNext(Unit.Default);
    }
    public void Bind(Subject<Unit> exitSceneSignalSubj)
    {
        _exitSceneSignalSubj = exitSceneSignalSubj;
    }    
}