using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Laberis.com.lairon.laberis.server.exception;
using Laberis.com.lairon.laberis.server.model;

namespace Laberis.com.lairon.laberis.server
{
    public class LaberisServer
    {
        private string addres;

        private HttpClient _client;

        public LaberisServer(string addres)
        {
            this.addres = addres;
            _client = new HttpClient()
            {
                BaseAddress = new Uri(addres)
            };
        }

        public async Task<User> Login(string login, string password)
        {
            string body = JsonSerializer.Serialize(new
            {
                login = login,
                password = password
            });
            StringContent stringContent = new StringContent(
                body,
                Encoding.UTF8,
                "application/json");
            

            HttpResponseMessage response = await _client.PostAsync("/user/login", stringContent);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<User>(responseBody);
                case HttpStatusCode.NotFound:
                    throw new UserNotFoundException();
                case (HttpStatusCode) 602:
                    throw new WrongPasswordException();
                case (HttpStatusCode) 500:
                    throw new ServerErrorException(response.StatusCode);
            }

            return null;
        }
        
        
        public async Task<User> Register(User user)
        {
            user.token = "";
            string body = JsonSerializer.Serialize(user);
            StringContent stringContent = new StringContent(body, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/user/register", stringContent);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<User>(responseBody);
                case (HttpStatusCode) 601:
                    throw new SimplePasswordException();
                case (HttpStatusCode) 603:
                    throw new LoginAlreadyExistsException();
            }

            return null;
        }
        
    }
    
    
    
}