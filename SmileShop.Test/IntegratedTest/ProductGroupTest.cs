

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmileShop.Test.IntegratedTest
{
    [TestClass]
    public class ProductGroupTest: TestBase
    {


        [TestMethod]
        public async Task GetAll_EmptyList()
        {

            var dbName = Guid.NewGuid().ToString();
            var factory =  BuildWebApplicationFactory(dbName);

            var client = factory.CreateClient();
            var url = "api/Products/groups";
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            Console.WriteLine(await response.Content.ReadAsStringAsync());

            var productGroup = JsonConvert.DeserializeObject<ServiceResponseWithPagination<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());
            Assert.IsNull(productGroup.Data);
        }
    }
}
