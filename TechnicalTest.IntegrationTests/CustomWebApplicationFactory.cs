using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TechnicalTest.Services.Services.Interfaces;
namespace TechnicalTest.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IProductService> ProductServiceMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace the real IProductService with a mock
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IProductService));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddSingleton(_ => ProductServiceMock.Object);
        });
    }
}
