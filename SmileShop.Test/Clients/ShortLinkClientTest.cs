using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SmileShop.Clients;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SmileShop.Test.Client
{
    [TestClass()]
    public class ShortLinkClientTest
    {
        [TestMethod()]
        public async Task ShortLinkClient_Test()
        {
            // Arrage

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues("Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJlMzlhNjA0ZS0xMmU5LTRkYzUtYWM2MS1lODU3MDQ0Yzg1NzQiLCJ1bmlxdWVfbmFtZSI6InNhbGVzZm9yY2UuRGV2Iiwicm9sZSI6IlZlbmRvciIsIlNvdXJjZSI6InZlbmRvci5DUk0uc2FsZXNmb3JjZSIsIm5iZiI6MTYyNDg1MDkwNiwiZXhwIjoxNjI0ODU0NTA2LCJpYXQiOjE2MjQ4NTA5MDZ9.WX6G-Rd26w3bTesCUb8B_mc_Upyd4N2-MRmmOSjsJJlan1VOT_SfLiesjIv-PgBJWNOMpcczwZLvELs0fhXktQ"));
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var client = new ShortLinkClient(mockHttpContextAccessor.Object, "http://uat.siamsmile.co.th:9220");

            // Act
            var response = await client.GenerateLinkShortenAsync("http://www.gooogle.com/");

            // Assert

            Assert.IsInstanceOfType(response, typeof(ShortLinkClient.ShortLinkResponse));
            Trace.WriteLine(JsonConvert.SerializeObject(response));
            Assert.IsNotNull(response.ShortURL);
        }
    }
}