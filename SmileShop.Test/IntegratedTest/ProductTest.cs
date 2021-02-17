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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmileShop.Test.IntegratedTest
{
    /**
     * Test Name : ProductTest
     * Created by : AkiAkira
     * Version 1.0 : 16 Feb 2021.
     * 
     * Test on /api/Products
     */

    [TestClass]
    public class ProductTest : TestBase
    {
        #region GetAll
        // GetAll_NoData_ReturnError
        [TestMethod]
        [TestCategory("GetAll")]
        public async Task GetAll_NoData_ReturnError()
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = "api/products";

            // ===== Act =====
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var product = JsonConvert.DeserializeObject<ServiceResponseWithPagination<ProductDTO>>(await response.Content.ReadAsStringAsync());
            Assert.IsNull(product.Data);
            Assert.IsTrue(product.Message.Contains("Product is not Exist"));

            Console.WriteLine(ClassToJsonString(product));
        }


        // GetAll_HaveData_ReturnData
        [DataTestMethod]
        [TestCategory("GetAll")]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetAll_HaveData_ReturnData(int page)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products?page={page}";

            // ===== Act ======
            var response = await client.GetAsync(url);


            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponseWithPagination<List<ProductDTO>>>(await response.Content.ReadAsStringAsync());

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmountRecords, 15);

            Console.WriteLine(ClassToJsonString(result));

            if (page == 1)
                Assert.AreEqual(result.Data.Count, 10);
            else
                Assert.AreEqual(result.Data.Count, 5);

        }


        // GetAll_WithFilter_ReturnFilterData
        [DataTestMethod]
        [TestCategory("GetAll")]
        [DataRow("A")]
        [DataRow("2")]
        public async Task GetAll_WithFilter_ReturnFilterData(string filter)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products?Name={filter}";

            // ===== Act ======
            var response = await client.GetAsync(url);


            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponseWithPagination<List<ProductDTO>>>(await response.Content.ReadAsStringAsync());

            Assert.IsTrue(result.IsSuccess);

            switch (filter)
            {
                case "A":
                    Assert.AreEqual(result.Data.Count, 3);
                    break;
                case "2":
                    Assert.AreEqual(result.Data.Count, 5);
                    break;
                    //default:
                    //    Assert.AreEqual(result.Data.Count, 15);
                    //    break;
            }
            Console.WriteLine(ClassToJsonString(result));


        }


        // GetAll_WithOrder_ReturnOrderedData
        [TestMethod]
        [TestCategory("GetAll")]
        public async Task GetAll_WithOrder_ReturnOrderedData() {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products?OrderBy=Name&sort=asd";

            // ===== Act ======
            var response = await client.GetAsync(url);


            // ===== Assert =====
            var result = JsonConvert.DeserializeObject<ServiceResponseWithPagination<List<ProductDTO>>>(await response.Content.ReadAsStringAsync());

            Assert.IsTrue(result.IsSuccess);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmountRecords, 15);
            Assert.AreEqual(result.Data[0].Name, "Test Product 1 Group A");
            Assert.AreEqual(result.Data[1].Name, "Test Product 1 Group B");
        }
        #endregion

        #region Get
        // Get_IdIsInvalid_RetureError // ArgumentOutOfRangeException
        [DataTestMethod]
        [TestCategory("Get")]
        [DataRow(-20)]
        [DataRow(-1)]
        public async Task Get_IdIsInvalid_RetureError(int id)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";


            // ===== Act ======
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("Id must be greater than 0"));
        }


        // Get_InvalidProduct_ReturnError // InvalidOperationException
        [DataTestMethod]
        [TestCategory("Get")]
        [DataRow(60)]
        [DataRow(50)]
        public async Task Get_InvalidProduct_ReturnError(int id)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";


            // ===== Act ======
            var response = await client.GetAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("Product is not Exist"));
        }


        // Get_ValidProduct_ReturnDataWithSameID
        [DataTestMethod]
        [TestCategory("Get")]
        [DataRow(10)]
        [DataRow(15)]
        public async Task Get_ValidProduct_ReturnDataWithSameID(int id)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";

            // ===== Act ======
            var response = await client.GetAsync(url);


            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());


            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(id, result.Data.Id);

            switch (id)
            {
                case 10:
                    Assert.AreEqual("Test Product 1 Group D", result.Data.Name);
                    break;
                case 15:
                    Assert.AreEqual("Test Product 3 Group E", result.Data.Name);
                    break;
            }
        }

        #endregion


        #region Add
        // Add_NoUser_ReturnError // UnauthorizedAccessException
        [DataTestMethod]
        [TestCategory("Add")]
        [DataRow(1, "New Product", "20")]
        [DataRow(2, "New Product", "20")]
        public async Task Add_NoUser_ReturnError(int gid, string n, string pf) {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            //var user = await SetupUser(context, mapper, httpContext.Object, new UserRegisterDto { Username = "Test", Password = "Test" });

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products";

            // Login
            //var bearer = await SimulateLogin(client);
            var price = Decimal.Parse(pf);

            var newProductGroup = new ProductAddDTO { GroupId = gid, Name = n, Price = price };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            // ===== Act ======
            var response = await client.PostAsync(url, httpContent);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("User must be presented to perform this method"));

            var assContext = BuildContext(dbName);

            var count = await assContext.Product.CountAsync();
            Assert.AreEqual(0, count);
        }


        // Add_InvalidProductGroup_ReturnError // InvalidOperationException
        [DataTestMethod]
        [TestCategory("Add")]
        [DataRow(-5, "New Product", "20")]
        [DataRow(25, "New Product", "20")]
        public async Task Add_InvalidProductGroup_ReturnError(int gid, string n, string pf)
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

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products";

            // Login
            var bearer = await SimulateLogin(client);
            var price = Decimal.Parse(pf);

            var newProductGroup = new ProductAddDTO { GroupId = gid, Name = n, Price = price };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            // ===== Act ======
            var response = await client.PostAsync(url, httpContent);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("Product Group is not Exist"));

            var assContext = BuildContext(dbName);

            var count = await assContext.Product.CountAsync();
            Assert.AreEqual(0, count);
        }


        // Add_ValidProduct_ReturnData
        [DataTestMethod]
        [TestCategory("Add")]
        [DataRow(1, "New Product 1 GROUP A", "50")]
        [DataRow(3, "New Product 2 GROUP C", "20")]
        public async Task Add_ValidProduct_ReturnData(int gid, string n, string pf)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products";

            // Login
            var bearer = await SimulateLogin(client);
            var price = Decimal.Parse(pf);

            var newProductGroup = new ProductAddDTO { GroupId = gid, Name = n, Price = price };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            // ===== Act ======
            var response = await client.PostAsync(url, httpContent);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(ClassToJsonString(result));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data.Name, n);

            var assContext = BuildContext(dbName);

            var count = await assContext.Product.CountAsync();
            Assert.AreEqual(1, count);
        }

        #endregion

        #region Edit
        // Edit_IdIsInvalid_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [TestCategory("Edit")]
        [DataRow(0, 1, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(-15, 1, "Edit Product 1 GROUP Z", "20.00")]
        public async Task Edit_IdIsInvalid_ReturnError(int id, int groupID, string editName, string editPrice)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            //var context = BuildContext(dbName);
            //var mapper = BuildMap();
            //var httpContext = new Mock<IHttpContextAccessor>();
            //var http = new DefaultHttpContext();
            //httpContext.Setup(_ => _.HttpContext).Returns(http);

            //var user = await SetupUser(context, mapper, httpContext.Object, new UserRegisterDto { Username = "Test", Password = "Test" });

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";

            // Login
            //var bearer = await SimulateLogin(client);
            var price = Decimal.Parse(editPrice);

            var newProductGroup = new ProductAddDTO { GroupId = groupID, Name = editName, Price = price };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            // ===== Act ======
            var response = await client.PutAsync(url, httpContent);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("Id must be greater than 0"));
        }


        // Edit_InvalidProduct_ReturnError //InvalidOperationException
        [DataTestMethod]
        [TestCategory("Edit")]
        [DataRow(1, -1, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(2, 99, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(18, 1, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(200, 5, "Edit Product 1 GROUP Z", "20.00")]
        public async Task Edit_InvalidProduct_ReturnError(int id, int groupID, string editName, string editPrice)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var editPriceD = Decimal.Parse(editPrice);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            var data = await context.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";

            // Login
            //var bearer = await SimulateLogin(client);
            var price = Decimal.Parse(editPrice);

            var newProductGroup = new ProductAddDTO { GroupId = groupID, Name = editName, Price = price };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            // ===== Act ======
            var response = await client.PutAsync(url, httpContent);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Message);

            var assContext = BuildContext(dbName);

            if (!(data is null))
            {
                var editData = await assContext.Product.Where(_ => _.Id == id).FirstAsync();

                Assert.AreEqual(data.GroupId, editData.GroupId);
                Assert.AreEqual(data.Name, editData.Name);
                Assert.AreEqual(data.Price, editData.Price);
            }
        }


        // Edit_ValidProduct_ReturnDataWithSaveChanged
        [DataTestMethod]
        [TestCategory("Edit")]
        [DataRow(1, 1, "Edit Product 1 GROUP A", "10.00")]
        [DataRow(4, 2, "Edit Product 2 GROUP B", "20.00")]
        [DataRow(10, 3, "Edit Product 3 GROUP C", "30.00")]
        [DataRow(13, 4, "Edit Product 4 GROUP D", "40.00")]
        public async Task Edit_ValidProduct_ReturnDataWithSaveChanged(int id, int groupID, string editName, string editPrice)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var editPriceD = Decimal.Parse(editPrice);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            var data = await context.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";

            // Login
            //var bearer = await SimulateLogin(client);
            var price = Decimal.Parse(editPrice);

            var newProductGroup = new ProductAddDTO { GroupId = groupID, Name = editName, Price = price };
            var httpContent = await ObjectToJsonContent(newProductGroup);

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            // ===== Act ======
            var response = await client.PutAsync(url, httpContent);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsNotNull(data);
            Assert.IsNotNull(result.Data);

            var assContext = BuildContext(dbName);

            var editData = await assContext.Product.Where(_ => _.Id == id).FirstAsync();

            Assert.AreEqual(result.Data.GroupId, editData.GroupId);
            Assert.AreEqual(result.Data.Name, editData.Name);
            Assert.AreEqual(result.Data.Price, editData.Price);

            var count = await assContext.Product.Where(_ => _.Name.Contains(editName)).CountAsync();

            Assert.AreEqual(1, count);
        }

        #endregion

        #region Delete
        // Delete_IdIsInvalid_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [TestCategory("Delete")]
        [DataRow(0)]
        [DataRow(-25)]
        [DataRow(18)]
        [DataRow(200)]
        public async Task Delete_IdIsInvalid_ReturnError(int id)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data 
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            var data = await context.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";

            // Login
            //var bearer = await SimulateLogin(client);


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{bearer}");

            // ===== Act ======
            var response = await client.DeleteAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            Assert.IsFalse(result.IsSuccess);

            if(id > 0)
                Assert.IsTrue(result.Message.Contains("Product is not Exist"));
            else
                Assert.IsTrue(result.Message.Contains("Id must be greater than 0"));
        }

        // Delete_ValidProduct_ReturnDataWithSaveChanged
        [DataTestMethod]
        [TestCategory("Delete")]
        [DataRow(1)]
        [DataRow(4)]
        [DataRow(10)]
        [DataRow(13)]
        public async Task Delete_ValidProduct_ReturnDataWithSaveChanged(int id)
        {

            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();

            // Generate Data
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            var data = await context.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            // Generate API & Client
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();
            var url = $"api/products/{id}";

            // ===== Act ======
            var response = await client.DeleteAsync(url);

            // ===== Assert =====
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ServiceResponse<ProductDTO>>(await response.Content.ReadAsStringAsync());

            var assContext = BuildContext(dbName);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);


            var deleteData = await assContext.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            Assert.IsNull(deleteData);

            var count = await assContext.Product.CountAsync();

            Assert.AreEqual(14, count);
        }
        #endregion
    }
}
