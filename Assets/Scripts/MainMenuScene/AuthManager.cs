using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance {get; private set;}
    private Client supabase;

     async void Awake()
    {
        try
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return; 
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            var options = new SupabaseOptions { AutoConnectRealtime = true };
            supabase = new Client(ClientInfo.CLIENT_URL, ClientInfo.CLIENT_KEY, options);
            await supabase.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.Log("Произошла непредвиденная ошибка, при инициализации менеджера регистрации: " + e);
        }
    }
    
    public Client GetClient() => supabase;
    
    public async Task<(bool success, string message)> SignUpAsync(string email, string password, string nickname)
    {
        try
        {
            var session = await supabase.Auth.SignUp(email, password, new Supabase.Gotrue.SignUpOptions
            {
                Data = new Dictionary<string, object> {{"nickname", nickname}}
            });
            if (session == null)
            {
                return(false, "Не удалось зарегистрироваться. Сервер не отвечает");
            }
            
            return (true, "Регистрация почти завершена! Проверьте вашу почту и подтвердите регистрацию аккаунта");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка регистрации: {ex.Message}");
            return (false, "Ошибка при регистрации: " + ex.Message);
        }
    }

    public async Task<(bool success, string message, PlayerData loadedPlayerData)> SignInAsync(string email,
        string password)
    {
        try
        {
            var session = await supabase.Auth.SignIn(email, password);
            if (session == null || session.User == null)
            {
                return (false, "Неверный email и/или пароль", null);
            }

            var loadedData = await DataManager.Instance.LoadData();
            if (loadedData == null)
            {
                return (false, "Не удалось загрузить данные пользователя", null);
            }
            return (true, "Вход успешен", loadedData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка входа: {ex.Message}");
            return(false, "Ошибка при входе в аккаунт: " + ex.Message, null);
        }
    }
}