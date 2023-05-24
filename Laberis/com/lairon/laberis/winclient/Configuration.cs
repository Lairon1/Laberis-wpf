using System.Linq;
using Laberis.com.lairon.laberis.server.model;
using Microsoft.Win32;

namespace Laberis
{
    public class Configuration
    {
        public static User loadCurrentUser()
        {
            RegistryKey userKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Laberis\\CurrentUser");
            string[] valueNames = userKey.GetValueNames();
            if (valueNames == null)
                return null;
            if (
                !valueNames.Contains("Login") ||
                !valueNames.Contains("Firstname") ||
                !valueNames.Contains("Lastname") ||
                !valueNames.Contains("Token")) return null;


            return new User(
                (string)userKey.GetValue("Login"),
                (string)userKey.GetValue("Firstname"),
                (string)userKey.GetValue("Lastname"),
                (string)userKey.GetValue("Token"),
                ""
            );
        }
        
        public static void saveCurrentUser(User user)
        {
            RegistryKey userKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Laberis\\CurrentUser");
            if (user == null)
            {
                userKey.DeleteValue("Login");
                userKey.DeleteValue("Firstname");
                userKey.DeleteValue("Lastname");
                userKey.DeleteValue("Token");
                return;
            }
            userKey.SetValue("Login", user.login);
            userKey.SetValue("Firstname", user.firstname);
            userKey.SetValue("Lastname", user.lastname);
            userKey.SetValue("Token", user.token);
        }
        
    }
}