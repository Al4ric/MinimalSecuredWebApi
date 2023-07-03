using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public class SimpleTest : IClassFixture<WebAppFactory>
    {
        private readonly HttpClient _httpClient;

        public SimpleTest(WebAppFactory factory)
        { 
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost/");
        }

        [Theory]
        [InlineData("5", "Hello #5")]
        [InlineData("1", "Hello #1")]
        [InlineData("3", "Hello #3")]
        public async Task SayHiToNumber(string number, string expectedResponse)
        {
            _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.UserId, number);

            var response = await _httpClient.GetStringAsync("hi");
            Assert.Equal(expectedResponse, response);
        }
    }

    public class WebAppFactory : WebApplicationFactory<Program>
    {
        private string DefaultUserId { get; set; } = "1";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = DefaultUserId);

                services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, _ => { });
            });
        }
    }
}