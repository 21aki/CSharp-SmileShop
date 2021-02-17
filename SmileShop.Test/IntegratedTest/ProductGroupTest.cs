

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SmileShop.DTOs;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmileShop.Test.IntegratedTest
{
    /**
     * Test Name : ProductGroupTest
     * Created by : AkiAkira
     * Version 1.0 : 15 Feb 2021.
     * 
     * Test on /api/Products/group
     */

    [TestClass]
    public class ProductGroupTest : TestBase
    {
        #region GetAll
        [TestMethod]
        [TestCategory("GetAll")]
        public async Task GetAll_EmptyList()
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = "api/products/groups";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var productGroup = JsonConvert.DeserializeObject<ServiceResponseWithPagination<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());
            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
        }

        [DataTestMethod]
        [TestCategory("GetAll")]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetAll_ReturnList(int page)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/groups?page={page}&RecordsPerPage=3";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var productGroup = JsonConvert.DeserializeObject<ServiceResponseWithPagination<List<ProductGroupDTO>>>(await response.Content.ReadAsStringAsync());

            switch (page)
            {
                case 1:
                    Assert.AreEqual(3, productGroup.Data.Count);
                    break;
                case 2:
                    Assert.AreEqual(2, productGroup.Data.Count);
                    break;
            }
        }

        [DataTestMethod]
        [TestCategory("GetAll")]
        [DataRow("Group 1")]
        [DataRow("Group 4")]
        public async Task GetAll_CanFilterByName_ReturnFilteredListOfProductGroup(String filter)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/groups?filter={filter}";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var productGroup = JsonConvert.DeserializeObject<ServiceResponseWithPagination<List<ProductGroupDTO>>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(1, productGroup.Data.Count);
            Assert.IsTrue(productGroup.Data[0].Name.Contains(filter));
        }

        [TestMethod]
        [TestCategory("GetAll")]
        public async Task GetAll_CanOrderByName_ReturnOrderedList()
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/groups?OrderBy=Id&Sort=desc";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var productGroup = JsonConvert.DeserializeObject<ServiceResponseWithPagination<List<ProductGroupDTO>>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(5, productGroup.Data.Count);
            Assert.AreEqual(5, productGroup.Data[0].Id);
            Assert.AreEqual(4, productGroup.Data[1].Id);
            Assert.AreEqual(3, productGroup.Data[2].Id);
            Assert.AreEqual(2, productGroup.Data[3].Id);
            Assert.AreEqual(1, productGroup.Data[4].Id);
        }

        #endregion

        #region Get
        [DataTestMethod]
        [TestCategory("Get")]
        [DataRow(-5)]
        [DataRow(0)]
        [DataRow(1)]
        public async Task Get_InvalidId_ReturnError(int id)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/groups/{id}";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
        }

        [TestMethod]
        [TestCategory("Get")]
        public async Task Get_HaveDataAndProductID_ReturnProductGroupWithSameId()
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/groups/99";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
        }


        [DataTestMethod]
        [TestCategory("Get")]
        [DataRow(1)]
        [DataRow(3)]
        public async Task Get_HaveDataAndProductID_ReturnProductGroupWithSameId(int id)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/groups/{id}";

            var data = await context.ProductGroup.Where(_ => _.Id == id).FirstAsync();

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsNotNull(productGroup.Data);
            Assert.IsTrue(productGroup.IsSuccess);
            Assert.AreEqual(data.Id, productGroup.Data.Id);
            Assert.AreEqual(data.Name, productGroup.Data.Name);
        }
        #endregion

        #region Add
        [TestMethod]
        [TestCategory("Add")]
        public async Task Add_NoLoginUser_ReturnErrorMessage()
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/groups";

            var newProductGroup = new ProductGroupAddDTO { Name = "New Product Group 1", Status = true };
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(newProductGroup));
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // ===== Act =====
            var response = await client.PostAsync(url, httpContent);

            // ===== Assert =====

            if (response.Content == null)
                Assert.Fail();

            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            //Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
            Assert.AreEqual("User must be presented to perform this method", productGroup.Message);
        }

        [TestMethod]
        [TestCategory("Add")]
        public async Task Add_SentBlankProductGroupName_ReturnErrorMessage()
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await SetupUser(context, mapper, httpContext.Object, new UserRegisterDto { Username = "Test", Password = "Test" });

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            // Login
            var bearer = await SimulateLogin(client);
            //Console.WriteLine(ClassToJsonString(baerer));

            var newProductGroup = new ProductGroupAddDTO { Name = "", Status = true };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            var url = $"api/products/groups";

            // ===== Act =====
            var response = await client.PostAsync(url, httpContent);

            // ===== Assert =====

            if (response.Content == null)
                Assert.Fail();

            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
            Assert.IsTrue(productGroup.Message.Contains("Please fill Product Group's Name"));
        }

        [TestMethod]
        [TestCategory("Add")]
        public async Task Add_WithData_ReturnAddedResult()
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await SetupUser(context, mapper, httpContext.Object, new UserRegisterDto { Username = "Test", Password = "Test" });

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            // Login
            var bearer = await SimulateLogin(client);
            //Console.WriteLine(ClassToJsonString(baerer));

            var newProductGroup = new ProductGroupAddDTO { Name = "New Product Group 1", Status = true };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            var url = $"api/products/groups";

            // ===== Act =====
            var response = await client.PostAsync(url, httpContent);

            // ===== Assert =====

            if (response.Content == null)
                Assert.Fail();

            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNotNull(productGroup.Data);
            Assert.IsTrue(productGroup.IsSuccess);
            Assert.AreEqual(newProductGroup.Name, productGroup.Data.Name);
        }

        #endregion

        #region Edit
        [TestMethod]
        [TestCategory("Edit")]
        public async Task Edit_SentBlankProductGroupName_ReturnErrorMessage()
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var newProductGroup = new ProductGroupAddDTO { Name = "", Status = true };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            var url = "/api/Products/groups/1";

            // ===== Act =====
            var response = await client.PutAsync(url, httpContent);

            // ===== Assert =====
            if (response.Content == null)
                Assert.Fail();

            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
            Assert.IsTrue(productGroup.Message.Contains("Please fill Product Group's Name"));
        }

        [TestMethod]
        [TestCategory("Edit")]
        public async Task Edit_ProductIdIsLessOrEqualZero_ReturnErrorMessage()
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var newProductGroup = new ProductGroupAddDTO { Name = "Test Product Group Edit 1", Status = true };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            var url = "/api/Products/groups/-11";

            // ===== Act =====
            var response = await client.PutAsync(url, httpContent);

            // ===== Assert =====
            if (response.Content == null)
                Assert.Fail();

            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
            Assert.IsTrue(productGroup.Message.Contains("Id must be greater than 0"));
        }

        [TestMethod]
        [TestCategory("Edit")]
        public async Task Edit_NoData_ReturnErrorMessage()
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var newProductGroup = new ProductGroupAddDTO { Name = "Test Product Group Edit 1", Status = true };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            var url = "/api/Products/groups/1";

            // ===== Act =====
            var response = await client.PutAsync(url, httpContent);

            // ===== Assert =====

            if (response.Content == null)
                Assert.Fail();

            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
            Assert.IsTrue(productGroup.Message.Contains("Product Group is not Exist"));
        }

        [TestMethod]
        [TestCategory("Edit")]
        public async Task Edit_WithData_ReturnEditedResult()
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            var productGroupId = 1;

            var dataProductGroup = await context.ProductGroup
                                                .Where(x => x.Id == productGroupId)
                                                .FirstOrDefaultAsync();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var newProductGroup = new ProductGroupAddDTO { Name = "Test Product Group Edit 1", Status = true };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            var url = $"/api/Products/groups/{productGroupId}";

            // ===== Act =====
            var response = await client.PutAsync(url, httpContent);

            // ===== Assert =====

            if (response.Content == null)
                Assert.Fail();

            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNotNull(productGroup.Data);
            Assert.IsTrue(productGroup.IsSuccess);
            Assert.AreEqual(newProductGroup.Name, productGroup.Data.Name);

            var count = await context.ProductGroup
                                                .Where(x => x.Name.Contains(newProductGroup.Name))
                                                .CountAsync();

            Assert.AreEqual(1, count);
        }
        #endregion

        #region Delete
        [TestMethod]
        [TestCategory("Delete")]
        [DataRow(4)]
        [DataRow(0)]
        [DataRow(-12)]
        public async Task Delete_NoData_ReturnErrorMessage(int productGroupId)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var url = $"/api/Products/groups/{productGroupId}";

            // ===== Act =====
            var response = await client.DeleteAsync(url);


            // ===== Assert =====

            if (response.Content == null)
                Assert.Fail();

            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNull(productGroup.Data);
            Assert.IsFalse(productGroup.IsSuccess);
        }


        [TestMethod]
        [TestCategory("Delete")]
        [DataRow(4)]
        [DataRow(3)]
        public async Task Delete_WithID_ReturnDeletedResult(int productGroupId)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            var dataCount = await context.ProductGroup.CountAsync();

            // Build API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var url = $"/api/Products/groups/{productGroupId}";

            // ===== Act =====
            var response = await client.DeleteAsync(url);


            // ===== Assert =====

            if (response.Content == null)
                Assert.Fail();

            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();

            var productGroup = JsonConvert.DeserializeObject<ServiceResponse<ProductGroupDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(productGroup));
            Assert.IsNotNull(productGroup.Data);
            Assert.IsTrue(productGroup.IsSuccess);
            Assert.AreEqual(productGroupId, productGroup.Data.Id);


            var assContext = BuildContext(dbName);
            var count = await context.ProductGroup.CountAsync();

            Assert.AreEqual(dataCount - 1, count);
        }

        #endregion
    }
}
