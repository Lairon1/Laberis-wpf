using Laberis.com.lairon.laberis.data;
using Laberis.com.lairon.laberis.server.model;

namespace Laberis.com.lairon.laberis.winclient
{
    public class Application
    {
        public static MainWindow MainWindow { get; set; }
        public static DataProvider LaberisServer { get; set; }
        public static User User { get; set; }
    }
}