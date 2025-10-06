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

    public async Task<(bool success, string message, PlayerData loadedPlayerData)> SignInAsync(string email,
        string password)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Интернет отсутствует");
            return OfflineSignIn(email, password);
        }
        try
        {
            var session = await _supabase.Auth.SignIn(email, password);
            if (session == null || session.User == null)
            {
                return (false, "Неверный email и/или пароль", null);
            }

            var authCache = new AuthCache{email = email, passwordHash = password.GetHashCode().ToString()};
            LocalSaveManager.SaveAuthCache(authCache);
            
            return (true, "Вход успешен", null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка входа: {ex.Message}. Попытка оффлайн-входа");
            return OfflineSignIn(email, password);
        }
    }

    private (bool success, string message, PlayerData loadePlayerData) OfflineSignIn(string email, string password)
    {
        var cachedAuth = LocalSaveManager.LoadAuthCache();
        var localProfile = LocalSaveManager.LoadProfile();
        
        if(cachedAuth != null && localProfile != null && cachedAuth.email.Equals(email, StringComparison.OrdinalIgnoreCase) && cachedAuth.passwordHash == password.GetHashCode().ToString())
        {
            return (true, "Вход в оффлайн-режим", localProfile);
        }
        
        return (false, "Вход в оффлайн-режим не удачен, проверьте данные", null);
    }
}