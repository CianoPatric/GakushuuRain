using NUnit.Framework;
using System.Reflection;

public class CoreArchitectureTests
{
    private DIContainer _rootContainer;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var gameEntryPoint = new GameEntryPoint();
        var container = typeof(GameEntryPoint).GetField("_rootContainer", BindingFlags.NonPublic | BindingFlags.Instance);
        _rootContainer = container?.GetValue(gameEntryPoint) as DIContainer;
    }

    [Test]
    public void T01_RootContainer_ShouldNotBeNull()
    {
        Assert.IsNotNull(_rootContainer, "Корневой DIContainer не был создан..");
    }

    [Test]
    public void T02_AuthManager_ShouldBeRegistered()
    {
        var authManager = _rootContainer.Resolve<AuthManager>();
        Assert.IsNotNull(authManager, "AuthManager не зарегистрирован в DIContainer");
    }

    [Test]
    public void T03_DataManager_ShouldBeRegistered()
    {
        var dataManager = _rootContainer.Resolve<DataManager>();
        Assert.IsNotNull(dataManager, "DataManager не зарегистирован в DIContainer");
    }

    [Test]
    public void T04_WordLibraryManager_ShouldBeRegistered()
    {
        var wordLibraryManager = _rootContainer.Resolve<WordLibraryManager>();
        Assert.IsNotNull(wordLibraryManager, "WordLibraryManager не зарегистирован DIContainer");
    }
}