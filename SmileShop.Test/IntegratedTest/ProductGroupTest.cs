using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //_factory.Server.Host.Start();
            //Console.WriteLine();
            var client = factory.CreateClient();
            var url = "​/api​/Products​/groups";
            var response = await client.GetAsync(url);

            Console.WriteLine(response);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceResponseWithPagination<List<ProductGroupDTO>>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
