using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Laberis.com.lairon.laberis.server.model;

namespace Laberis.com.lairon.laberis.data
{
    public interface DataProvider
    {

        Task<User> Login(string login, string password);
        Task<User> Register(User user);
        Task<bool> checkSession(User user);
        Task<Collection<Product>> getAllProducts();
        Task<Product> saveProduct(User user, Product product);
        Task SaveImage(User user, long id, string filePath);
        void loadImage(Product product);
        void deleteProduct(User user, Product product);
        
    }
}