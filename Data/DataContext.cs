using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class DataContext : DbContext    

    // code work:  docker run --name sqlserver --hostname sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=1q2w3e4r!@#$" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
    {
        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
        }

        public DbSet<Product> Products { get; set;}
        public DbSet<Category> Categories { get; set;}
        public DbSet<User> Users { get; set;}
    }
}