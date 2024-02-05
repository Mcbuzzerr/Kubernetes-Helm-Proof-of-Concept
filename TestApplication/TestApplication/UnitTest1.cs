using Newtonsoft.Json;
using System;
using System.IO;

namespace TestApplication
{
    public class UnitTest1
    {

        private readonly HttpClient _client;

        public UnitTest1()
        {
            // Setup HttpClient
            _client = new HttpClient();
        }
        
        [Fact]
        [Trait("Ready", "True")]
        public async Task TestHelloWorldEndpoint()
        {
            // Arrange
            var url = "http://" + Environment.GetEnvironmentVariable("TEST_TARGET_SERVICE_NAME") +":" + Environment.GetEnvironmentVariable("TEST_TARGET_SERVICE_PORT") + "/HelloWorld";

            // Act
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

            // Assert
            Assert.NotNull(dictionary);
            Assert.Equal("World", dictionary["Hello"]);
        }

        [Fact]
        [Trait("Ready", "False")]
        public void TestLawsOfMathematicsHasNotChanged()
        {
            int first_number = 3;
            int second_number = 4;
            int sum = first_number + second_number;
            Assert.Equal(7, sum);
        } 

    }
}