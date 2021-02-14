using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Models;
using SmileShop.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmileShop.Test
{
    /**
     * Test Name : ProductServiceTest
     * Created by : AkiAkira
     * Version 1.0 : 12 Feb 2021.
     * 
     * Test on ProductService
     */

    [TestClass]
    public class ProductTest : TestBase
    {
        // GetAll_NoData_ReturnError // InvalidOperationException
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetAll_NoData_ReturnError()
        {
            // ===== Arrage =====

            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act ======
            var service = new ProductServices(context, mapper, httpContext.Object);
            await service.GetAll(new PaginationDto(), new ProductFilterDTO(), new DataOrderDTO());

            // ===== Assert ======
            // Expect Exception
        }

        // GetAll_HaveData_ReturnData
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetAll_HaveData_ReturnData(int page)
        {

            // ===== Arrage =====

            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var accContext = BuildContext(dbName);
            var service = new ProductServices(accContext, mapper, httpContext.Object);
            var result = await service.GetAll(new PaginationDto { Page = page }, new ProductFilterDTO(), new DataOrderDTO());

            // ===== Assert =====
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmountRecords, 15);

            if (page == 1)
                Assert.AreEqual(result.Data.Count, 10);
            else
                Assert.AreEqual(result.Data.Count, 5);

        }

        // GetAll_WithFilter_ReturnFilterData
        [DataTestMethod]
        [DataRow("A")]
        [DataRow("2")]
        public async Task GetAll_WithFilter_ReturnFilterData(string filter)
        {

            // ===== Arrage =====

            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var accContext = BuildContext(dbName);
            var service = new ProductServices(accContext, mapper, httpContext.Object);
            var result = await service.GetAll(new PaginationDto { }, new ProductFilterDTO { Name = filter }, new DataOrderDTO());

            // ===== Assert =====
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

        }

        // GetAll_WithOrder_ReturnOrderedData
        [TestMethod]
        public async Task GetAll_WithOrder_ReturnOrderedData()
        {
            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var accContext = BuildContext(dbName);

            var service = new ProductServices(accContext, mapper, httpContext.Object);
            var result = await service.GetAll(new PaginationDto { }, new ProductFilterDTO(), new DataOrderDTO { OrderBy = "Name", Sort = "asd" });

            // ===== Assert =====
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.TotalAmountRecords, 15);
            Assert.AreEqual(result.Data[0].Name, "Test Product 1 Group A");
            Assert.AreEqual(result.Data[1].Name, "Test Product 1 Group B");

        }

        // Get_IdIsInvalid_RetureError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(-20)]
        [DataRow(-1)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Get_IdIsInvalid_RetureError(int id)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            //await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var service = new ProductServices(context, mapper, httpContext.Object);
            var result = await service.Get(id);

            // ===== Assert =====
            // Expect Exception

        }

        // Get_InvalidProduct_ReturnError // InvalidOperationException
        [DataTestMethod]
        [DataRow(60)]
        [DataRow(50)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Get_InvalidProduct_ReturnError(int id)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            //await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var service = new ProductServices(context, mapper, httpContext.Object);
            var result = await service.Get(id);

            // ===== Assert =====
            // Expect Exception
        }

        // Get_ValidProduct_ReturnDataWithSameID
        [DataTestMethod]
        [DataRow(10)]
        [DataRow(15)]
        public async Task Get_ValidProduct_ReturnDataWithSameID(int id)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var accContext = BuildContext(dbName);

            var service = new ProductServices(accContext, mapper, httpContext.Object);
            var result = await service.Get(id);

            // ===== Assert =====
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

        // Add_NoUser_ReturnError // UnauthorizedAccessException
        [DataTestMethod]
        [DataRow(1, "New Product", "20")]
        [DataRow(2, "New Product", "20")]
        public async Task Add_NoUser_ReturnError(int gid, string n, string pf)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var p = Decimal.Parse(pf, CultureInfo.InvariantCulture);

            bool exException = false;

            // ===== Act ======
            try
            {
                var service = new ProductServices(context, mapper, httpContext.Object);
                var result = await service.Add(new ProductAddDTO { GroupId = gid, Name = n, Price = p });
            }
            catch (UnauthorizedAccessException)
            {
                exException = true;
            }
            catch (Exception)
            {
                throw;
            }

            // ===== Assert =====
            // Expect Exception

            var assContext = BuildContext(dbName);

            Assert.IsTrue(exException);

            var count = assContext.Product.Count();
            Assert.AreEqual(0, count);

        }

        // Add_InvalidProductGroup_ReturnError // InvalidOperationException
        [DataTestMethod]
        [DataRow(-5, "New Product", "20")]
        [DataRow(25, "New Product", "20")]
        public async Task Add_InvalidProductGroup_ReturnError(int gid, string n, string pf)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_ProductGroup_Data(context, mapper, httpContext.Object);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            var p = Decimal.Parse(pf, CultureInfo.InvariantCulture);

            bool exException = false;

            // ===== Act ======
            var accContext = BuildContext(dbName);

            try
            {
                var service = new ProductServices(accContext, mapper, loginHttpContext.Object);
                var result = await service.Add(new ProductAddDTO { GroupId = gid, Name = n, Price = p });
            }
            catch (InvalidOperationException)
            {
                exException = true;
            }
            catch (Exception)
            {
                throw;
            }

            // ===== Assert =====
            // Expect Exception

            var assContext = BuildContext(dbName);

            Assert.IsTrue(exException);

            var count = assContext.Product.Count();
            Assert.AreEqual(0, count);
        }

        // Add_ValidProduct_ReturnData
        [DataTestMethod]
        [DataRow(1, "New Product 1 GROUP A", "50")]
        [DataRow(3, "New Product 2 GROUP C", "20")]
        public async Task Add_ValidProduct_ReturnData(int gid, string n, string pf)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Product_Data(context, mapper, httpContext.Object);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            var p = Decimal.Parse(pf, CultureInfo.InvariantCulture);

            // ===== Act ======

            var actContext = BuildContext(dbName);

            var service = new ProductServices(actContext, mapper, loginHttpContext.Object);
            var result = await service.Add(new ProductAddDTO { GroupId = gid, Name = n, Price = p });
            var pid = result.Data.Id;

            // ===== Assert =====
            var assContext = BuildContext(dbName);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(n, result.Data.Name);

            var assProduct = assContext.Product.Where(_ => _.Id == pid).FirstOrDefault();

            Assert.IsNotNull(assProduct);
            Assert.AreEqual(16, await assContext.Product.CountAsync());
        }

        // Edit_IdIsInvalid_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(0, 1, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(-15, 1, "Edit Product 1 GROUP Z", "20.00")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Edit_IdIsInvalid_ReturnError(int id, int groupID, string editName, string editPrice)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var editPriceD = Decimal.Parse(editPrice);

            //await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var service = new ProductServices(context, mapper, httpContext.Object);
            var result = await service.Edit(id, new ProductAddDTO { GroupId = groupID, Name = editName, Price = editPriceD });

            // ===== Assert =====
            // Expect Exception

        }

        // Edit_InvalidProduct_ReturnError //InvalidOperationException
        [DataTestMethod]
        [DataRow(1, -1, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(2, 99, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(18, 1, "Edit Product 1 GROUP Z", "20.00")]
        [DataRow(200, 5, "Edit Product 1 GROUP Z", "20.00")]
        public async Task Edit_InvalidProduct_ReturnError(int id, int groupID, string editName, string editPrice)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var editPriceD = Decimal.Parse(editPrice);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            bool exException = false;

            var data = await context.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            // ===== Act ======
            var actContext = BuildContext(dbName);

            try
            {
                var service = new ProductServices(actContext, mapper, httpContext.Object);
                var result = await service.Edit(id, new ProductAddDTO { GroupId = groupID, Name = editName, Price = editPriceD });
            }
            catch (InvalidOperationException)
            {
                exException = true;
            }
            catch (Exception)
            {
                throw;
            }

            // ===== Assert =====
            // Expect Exception
            var assContext = BuildContext(dbName);

            Assert.IsTrue(exException);

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
        [DataRow(1, 1, "Edit Product 1 GROUP A", "10.00")]
        [DataRow(4, 2, "Edit Product 2 GROUP B", "20.00")]
        [DataRow(10, 3, "Edit Product 3 GROUP C", "30.00")]
        [DataRow(13, 4, "Edit Product 4 GROUP D", "40.00")]
        public async Task Edit_ValidProduct_ReturnDataWithSaveChanged(int id, int groupID, string editName, string editPrice)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var editPriceD = Decimal.Parse(editPrice);

            await Generate_Product_Data(context, mapper, httpContext.Object);


            var data = await context.Product.Where(_ => _.Id == id).FirstAsync();

            var result = new ServiceResponse<ProductDTO>();

            // ===== Act ======
            var actContext = BuildContext(dbName);

            var service = new ProductServices(actContext, mapper, httpContext.Object);
            result = await service.Edit(id, new ProductAddDTO { GroupId = groupID, Name = editName, Price = editPriceD });

            // ===== Assert =====
            // Expect Exception
            var assContext = BuildContext(dbName);

            Assert.IsNotNull(data);
            Assert.IsNotNull(result.Data);

            var editData = await assContext.Product.Where(_ => _.Id == id).FirstAsync();

            Assert.AreEqual(result.Data.Group.Id, editData.GroupId);
            Assert.AreEqual(result.Data.Name, editData.Name);
            Assert.AreEqual(result.Data.Price, editData.Price);

            var count = await assContext.Product.Where(_ => _.Name.Contains(editName)).CountAsync();

            Assert.AreEqual(1, count);
        }

        // Delete_IdIsInvalid_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-25)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Delete_IdIsInvalid_ReturnError(int id)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            //await Generate_Product_Data(context, mapper, httpContext.Object);

            // ===== Act ======
            var service = new ProductServices(context, mapper, httpContext.Object);
            var result = await service.Delete(id);

            // ===== Assert =====
            // Expect Exception

        }

        // Delete_InvalidProduct_ReturnError //InvalidOperationException
        [DataTestMethod]
        [DataRow(18)]
        [DataRow(200)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Delete_InvalidProduct_ReturnError(int id)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);

            var data = await context.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            // ===== Act ======
            var actContext = BuildContext(dbName);

            var service = new ProductServices(actContext, mapper, httpContext.Object);
            var result = await service.Delete(id);

            // ===== Assert =====
            // Expect Exception
        }


        // Delete_ValidProduct_ReturnDataWithSaveChanged
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        [DataRow(10)]
        [DataRow(13)]
        public async Task Delete_ValidProduct_ReturnDataWithSaveChanged(int id)
        {

            // ===== Arrage =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Product_Data(context, mapper, httpContext.Object);


            var result = new ServiceResponse<ProductDTO>();

            // ===== Act ======
            var actContext = BuildContext(dbName);

            var service = new ProductServices(actContext, mapper, httpContext.Object);
            result = await service.Delete(id);

            // ===== Assert =====
            // Expect Exception
            var assContext = BuildContext(dbName);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.IsSuccess);

            var deleteData = await assContext.Product.Where(_ => _.Id == id).FirstOrDefaultAsync();

            Assert.IsNull(deleteData);

            var count = await assContext.Product.CountAsync();

            Assert.AreEqual(14, count);
        }

    }
}
