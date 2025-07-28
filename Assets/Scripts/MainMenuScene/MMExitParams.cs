public class MMExitParams 
{
    public GSEnterParams GameSceneEnterParams { get; }
    public MMExitParams(GSEnterParams gameSceneEnterParams)
    {
        GameSceneEnterParams = gameSceneEnterParams;
    }
}