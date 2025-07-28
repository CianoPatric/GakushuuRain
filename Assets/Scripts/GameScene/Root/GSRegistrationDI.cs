public static class GSRegistrationDI
{
    public static void Register(DIContainer container, GSEnterParams gameEnterParams)
    {
        container.RegisterInstance(gameEnterParams);
    }
}