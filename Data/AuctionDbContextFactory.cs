using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuctionPoc.Data;

public class AuctionDbContextFactory : IDesignTimeDbContextFactory<AuctionDbContext>
{
    public AuctionDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var keyVaultUri = configuration["KeyVault:VaultUri"];
        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential())
                .Build();
        }

        var connectionString = configuration.GetConnectionString("AuctionDatabase");
        if (!string.IsNullOrEmpty(connectionString))
        {
            var password = configuration["auction-admin"];
            if (!string.IsNullOrEmpty(password))
            {
                connectionString = connectionString.Replace("{KEYVAULT-PASSWORD}", password);
            }
        }

        var optionsBuilder = new DbContextOptionsBuilder<AuctionDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        return new AuctionDbContext(optionsBuilder.Options);
    }
}
