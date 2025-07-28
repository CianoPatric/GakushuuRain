using R3;
using UnityEngine;

public class MMRootBinder: MonoBehaviour
{
    private Subject<Unit> _exitSceneSignalSubj;
    public void HandleGoToGameButtonClick()
    {
        _exitSceneSignalSubj?.OnNext(Unit.Default);
    }
    public void Bind(Subject<Unit> exitSceneSignalSubj)
    {
        _exitSceneSignalSubj = exitSceneSignalSubj;
    }
}