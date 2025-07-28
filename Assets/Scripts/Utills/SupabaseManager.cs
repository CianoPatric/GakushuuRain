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
            //if (!IsValidEmail(email))
            {
                //if (!IsStrongPassword(password))
                {
                    //if (!ArePasswordsMatching(password, password2))
                    {
                        var session = await supabase.Auth.SignUp(email, password);
                        //if (session != null && session.User != null)
                        {
                            //UsersTable user = new()
                            {
                                //Id = session.User.Id,
                                //Username = UserNameField.GetComponent<TextMeshProUGUI>().text,
                                //AvatarData = StandartJsonAvatarData.ToString()
                            };
                            //var create = await UsersTable.CreateUser(client, user);
                            //if (create != true) {Debug.Log("Регистрация прошла с ошибкой");}
                        }
                        //else {Debug.Log("Регистрация не удалась, сервер не доступен");}   
                    }
                    //else {Debug.Log("Пароль должен совпадать");}
                }
                //else {Debug.Log("Пароль должен быть не менее 8 символов и содержать хотя бы 1 строчную и 1 заглавную букву, 1 цифру и 1 спецсимвол");}
            }
            //else {Debug.Log("Введите корректный email");}
        }
        catch (Exception ex)
        {
            Debug.Log("Непредвиденная ситуация при регистрации, код ошибки: " + ex.Message);
        }
    }
}