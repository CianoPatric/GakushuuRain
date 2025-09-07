using System;
using TMPro;
using UnityEngine;

public class SupabaseManager: MonoBehaviour
{
    public static string url = ClientInfo.CLIENT_URL;
    public static string key = ClientInfo.CLIENT_KEY;

    public TMP_InputField REmailField;
    public TMP_InputField RPasswordField;
    public TMP_InputField Password2Field;
    
        
    static Supabase.SupabaseOptions options = new()
    {
        AutoConnectRealtime = true
    };

    Supabase.Client supabase = new(url, key, options);
    public async void Awake()
    {
        try
        {
            await supabase.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.Log("При инициации Supabase, произошла ошибка:" + e.Message);
        }
    }
    public async void RegUser()
    {
        try
        {
            string email = REmailField.text;
            string password = RPasswordField.text;
            var session = await supabase.Auth.SignUp(email, password);

        }
        catch (Exception ex)
        {
            Debug.Log("Непредвиденная ситуация при регистрации, код ошибки: " + ex.Message);
        }
    }
}