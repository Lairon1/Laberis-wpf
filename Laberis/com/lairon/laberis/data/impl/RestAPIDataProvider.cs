using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Laberis.com.lairon.laberis.server.exception;
using Laberis.com.lairon.laberis.server.model;

namespace Laberis.com.lairon.laberis.data
{
    public class RestAPIDataProvider : DataProvider
    {
        private string addres;

        private HttpClient _client;

        public RestAPIDataProvider(string addres)
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
                case (HttpStatusCode)602:
                    throw new WrongPasswordException();
                case (HttpStatusCode)500:
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
                case (HttpStatusCode)601:
                    throw new SimplePasswordException();
                case (HttpStatusCode)603:
                    throw new LoginAlreadyExistsException();
            }

            return null;
        }

        public async Task<bool> checkSession(User user)
        {
            StringContent content = new StringContent(user.token, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/user/checksession", content);
            if (response.StatusCode == HttpStatusCode.OK) return true;
            return false;
        }

        public async Task<Collection<Product>> getAllProducts()
        {
            HttpResponseMessage response = await _client.GetAsync("/product/all");
            string content = await response.Content.ReadAsStringAsync();
            Collection<Product> allProducts = JsonSerializer.Deserialize<Collection<Product>>(content);

            foreach (Product product in allProducts)
            {
                loadImage(product);
            }

            return allProducts;
        }

        public async Task<Product> saveProduct(User user, Product product)
        {
            string serialize = JsonSerializer.Serialize(new
            {
                id = product.id,
                title = product.title,
                description = product.description,
                price = product.price,
                type = product.productType.ToString(),
                specifications = product.specifications
            });

            HttpResponseMessage httpResponseMessage =
                await _client.PostAsync($"/product/saveProduct?token={user.token}",
                    new StringContent(serialize, Encoding.UTF8, "application/json"));

            if (httpResponseMessage == null)
                throw new ServerException();

            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    string readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Product>(readAsStringAsync);
                case (HttpStatusCode)600:
                    throw new DontHavePermissionsException();
                default:
                    throw new ServerException();
            }
        }

        public async Task SaveImage(User user, long id, String filePath)
        {
            string url = $"/image/upload?token={user.token}&id={id}";

            MultipartFormDataContent formData = new MultipartFormDataContent();
            FileStream fileStream = File.OpenRead(filePath);

            StreamContent imageContent = new StreamContent(fileStream);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(imageContent, "file", Path.GetFileName(filePath));


            HttpResponseMessage response = await _client.PostAsync(url, formData);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
                    throw new ProductNotFoundException();
                case (HttpStatusCode)600:
                    throw new DontHavePermissionsException();
            }
        }


        public async void loadImage(Product product)
        {
            HttpResponseMessage response = await _client.GetAsync("/image/get?id=" + product.id);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            byte[] readAsByteArrayAsync = await response.Content.ReadAsByteArrayAsync();

            product.image = BitmapSourceFromByteArray(readAsByteArrayAsync);
        }


        public static BitmapSource BitmapSourceFromByteArray(byte[] buffer)
        {
            var bitmap = new BitmapImage();

            using (var stream = new MemoryStream(buffer))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            bitmap.Freeze(); 
            return bitmap;
        }

        public async void deleteProduct(User user, Product product)
        {
            HttpResponseMessage response = await _client.GetAsync($"product/delete?token={user.token}&id={product.id}");
            if (response.StatusCode == HttpStatusCode.OK)
                return;
            if (response.StatusCode == (HttpStatusCode)600)
                throw new DontHavePermissionsException();
        }
    }
}