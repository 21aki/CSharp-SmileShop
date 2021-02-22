using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Threading.Tasks;

namespace SmileShop.Test.IntegratedTest
{
    /**
     * Test Name : StockTest
     * Created by : AkiAkira
     * Version 1.0 : 16 Feb 2021.
     * 
     * Test on /api/Products/{id}/stock
     */

    [TestClass]
    public class StockTest : TestBase
    {

        #region Get
        // Get_InvalidProductID_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        public async Task Get_InvalidProductID_ReturnError(int id)
        {

            // ===== Arrange =====

            var dbName = Guid.NewGuid().ToString();
            // var context = BuildContext(dbName);
            // var mapper = BuildMap();
            // var httpContext = new Mock<IHttpContextAccessor>();
            // var http = new DefaultHttpContext();
            // httpContext.Setup(_ => _.HttpContext).Returns(http);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}/stock";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductStockDTO>>(await response.Content.ReadAsStringAsync());
            Assert.IsNull(result.Data);
            Assert.IsTrue(result.Message.Contains("Product ID must greater than 0"));
        }


        // Get_NoProductInDB_ReturnError // InvalidOperationException
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(5)]
        public async Task Get_NoProductInDB_ReturnError(int id)
        {

            // ===== Arrange =====

            var dbName = Guid.NewGuid().ToString();
            //// var context = BuildContext(dbName);
            //// var mapper = BuildMap();
            //// var httpContext = new Mock<IHttpContextAccessor>();
            //// var http = new DefaultHttpContext();
            //// httpContext.Setup(_ => _.HttpContext).Returns(http);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}/stock";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductStockDTO>>(await response.Content.ReadAsStringAsync());
            Assert.IsNull(result.Data);
            Assert.IsTrue(result.Message.Contains("Product is not Exist"));
        }

        // Get_ValidProductButNoStockHistory_ReturnWarningMessage
        [TestMethod]
        public async Task Get_ValidProductButNoStockHistory_ReturnWarningMessage()
        {

            // ===== Arrange =====

            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Stock_Data(context, mapper, httpContext.Object);
            var data = await context.Product.FindAsync(7);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/7/stock";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductStockDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Data);
            Assert.AreEqual($"Product ({data.Name}) have no recent stock records", result.Message);
        }


        #endregion


    }
}
