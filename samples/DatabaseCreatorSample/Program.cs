using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using DatabaseCreatorSample.Data;

namespace DatabaseCreatorSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //var model = SceneModel.Build();

            Console.WriteLine("Model load");

            //DatabaseBuild(model);

            Console.WriteLine("Database build");
        }

        static void DatabaseBuild(SceneModel model)
        {       
            using FootprintViewerDbContext db = new FootprintViewerDbContext(GetOptions());

            db.AddModel(model);
        }

        static DbContextOptions<FootprintViewerDbContext> GetOptions()
        {
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("DefaultConnection");
            var major = int.Parse(config["PostgresVersionMajor"]);
            var minor = int.Parse(config["PostgresVersionMinor"]);

            var optionsBuilder = new DbContextOptionsBuilder<FootprintViewerDbContext>();
            var options = optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.SetPostgresVersion(new Version(major, minor));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }
    }
}
