using System.Text.Json.Serialization;

namespace Laberis.com.lairon.laberis.server.model
{
    public class User
    {
        [JsonInclude] public string login { get; set; }
        [JsonInclude] public string firstname { get; set; }
        [JsonInclude] public string lastname { get; set; }
        [JsonInclude] public string token { get; set; }
        [JsonInclude] public string password { get; set; }

        public User(string login, string firstname, string lastname, string token, string password)
        {
            this.login = login;
            this.firstname = firstname;
            this.lastname = lastname;
            this.token = token;
            this.password = password;
        }

        public User()
        {
        }


        public override string ToString()
        {
            return "token = " + token;
        }
    }
}