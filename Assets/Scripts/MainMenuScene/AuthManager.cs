using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private Client _supabase;
    
     async void Awake()
    {
        try
        {
            DontDestroyOnLoad(gameObject);
            var options = new SupabaseOptions { AutoConnectRealtime = true };
            _supabase = new Client(ClientInfo.CLIENT_URL, ClientInfo.CLIENT_KEY, options);
            await _supabase.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.Log("Произошла непредвиденная ошибка, при инициализации менеджера регистрации: " + e);
        }
    }
    
    public Client GetClient() => _supabase;
    
    public async Task<(bool success, string message)> SignUpAsync(string email, string password, string nickname)
    {
        try
        {
            var session = await _supabase.Auth.SignUp(email, password, new Supabase.Gotrue.SignUpOptions
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

    public async Task<(bool success, string message)> SignInAsync(string email,
        string password)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return (false, "Нет подключения к интернету");
        }
        try
        {
            var session = await _supabase.Auth.SignIn(email, password);
            if (session == null || session.User == null)
            {
                return (false, "Неверный email и/или пароль");
            }

            var authCache = new AuthCache{email = email, passwordHash = password.GetHashCode().ToString()};
            LocalSaveManager.SaveAuthCache(authCache);
            
            return (true, "Вход успешен");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка входа: {ex.Message}");
            return (false, "Ошибка входа: " + ex.Message);
        }
    }

    public bool IsUserLoggedIn()
    {
        return _supabase.Auth.CurrentUser != null;
    }
}