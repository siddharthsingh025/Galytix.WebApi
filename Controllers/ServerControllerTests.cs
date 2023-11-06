using Galytix.WebApi.Controllers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Galytix.WebApi.Tests
{
    [TestFixture]
    public class ServerControllerTests
    {
        [Test]
        public async Task TestGetAverageGwpByLobAsync()
        {
            // Arrange
            var controller = new ServerController();
            var request = new GwpRequest
            {
                Country = "ae",
                Lob = new List<string> { "property", "transport" }
            };

            // Act
            var result = await controller.GetAverageGwpByLobAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Dictionary<string, decimal>>(result.Value);
            Assert.IsTrue(result.Value.ContainsKey("property"));
            Assert.IsTrue(result.Value.ContainsKey("transport"));
        }
    }
}
