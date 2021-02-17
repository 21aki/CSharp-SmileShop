using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmileShop.DTOs;
using SmileShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmileShop.Test.UnitTest
{
    /**
     * Test Name : StockServiceTest
     * Created by : AkiAkira
     * Version 1.0 : 15 Feb 2021.
     * 
     * Test on StockService
     */

    [TestClass]
    public class StockServiceTest : TestBase
    {

        #region Get
        // Get_InvalidProductID_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Get_InvalidProductID_ReturnError(int id)
        {

            // ===== Arrange =====

            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act ======
            var service = new StockServices(context, mapper, httpContext.Object);
            await service.Get(id);

            // ===== Assert ======
            // Expected Exception
        }

        // Get_NoProductInDB_ReturnError // InvalidOperationException
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Get_NoProductInDB_ReturnError(int id)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act =====
            var service = new StockServices(context, mapper, httpContext.Object);
            await service.Get(id);

            // ===== Assert =====
            // Expected Exception
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
            var data =  await context.Product.FindAsync(7);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, httpContext.Object);
            var result = await service.Get(7);

            // ===== Assert =====
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Data);
            Assert.AreEqual($"Product ({data.Name}) have no recent stock records", result.Message);

        }
        // Get_ValidProductWithStock_ReturnLastStockData
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(5)]
        public async Task Get_ValidProductWithStock_ReturnLastStockData(int id)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Stock_Data(context, mapper, httpContext.Object);
            var data = await context.Product.FindAsync(id);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, httpContext.Object);
            var result = await service.Get(id);

            // ===== Assert =====
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual($"Product ({data.Name}) requested record successfully", result.Message);
        }
        #endregion

        #region GetStockHistory
        // GetStockHistory_InvalidProductID_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task GetStockHistory_InvalidProductID_ReturnError(int id)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act =====
            var service = new StockServices(context, mapper, httpContext.Object);
            await service.GetStockHistory(id);

            // ===== Assert =====
            // Expected Exception
        }

        // GetStockHistory_NoProductInDB_ReturnError // InvalidOperationException
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetStockHistory_NoProductInDB_ReturnError(int id)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act =====
            var service = new StockServices(context, mapper, httpContext.Object);
            await service.GetStockHistory(id);

            // ===== Assert =====
            // Expected Exception
        }

        // GetStockHistory_ValidProductButNoStockHistory_ReturnWarningMessage
        [TestMethod]
        public async Task GetStockHistory_ValidProductButNoStockHistory_ReturnWarningMessage()
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

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(context, mapper, httpContext.Object);
            var result = await service.GetStockHistory(7, new PaginationDto());

            // ===== Assert =====
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0,result.Data.Count);
            Assert.AreEqual($"Product ({data.Name}) have no recent stock records", result.Message);
        }

        // GetStockHistory_ValidProductWithStock_ReturnStockHistoryData
        [DataTestMethod]
        [DataRow(2)]
        [DataRow(9)]
        public async Task GetStockHistory_ValidProductWithStock_ReturnStockHistoryData(int id)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_Stock_Data(context, mapper, httpContext.Object);
            var data = await context.Product.FindAsync(id);
            var stockData = await context.Stock.Where(_ => _.ProductId == id).CountAsync();

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(context, mapper, httpContext.Object);
            var result = await service.GetStockHistory(id, new PaginationDto());

            // ===== Assert =====
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual($"Product ({data.Name}) requested record successfully", result.Message);
            Assert.AreEqual(stockData, result.Data.Count());
        }
        #endregion

        #region Set
        // Set_NoUserPresented_ReturnError // UnauthorizedAccessException
        [DataTestMethod]
        [DataRow(1, 5, 0 , "Test")]
        [DataRow(5, 0, 5 , "Test")]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task Set_NoUserPresented_ReturnError(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act =====
            var service = new StockServices(context, mapper, httpContext.Object);
            await service.Set(id, new ProductStockAddDTO {Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====
            // Expected Exception
        }

        // Set_StockNumberIsBothZero_ReturnError // ArgumentNullException
        [DataTestMethod]
        [DataRow(1, 0, 0, "Test")]
        [DataRow(5, 0, 0, "Test")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Set_StockNumberIsBothZero_ReturnError(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, loginHttpContext.Object);
            await service.Set(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====
            // Expected Exception
        }

        // Set_OneOfEachStockNumberIsLessThanZero_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(1, -50, 0, "Test")]
        [DataRow(5, 0, -20, "Test")]
        [DataRow(9, 0, -10, "Test")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Set_OneOfEachStockNumberIsLessThanZero_ReturnError(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, loginHttpContext.Object);
            await service.Set(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====
            // Expected Exception
        }

        // Set_BothSideOfStockNumberHasValue_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(1, 10, 5, "Test")]
        [DataRow(5, 5, 10, "Test")]
        [DataRow(9, 99, 10, "Test")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Set_BothSideOfStockNumberHasValue_ReturnError(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, loginHttpContext.Object);
            await service.Set(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====
            // Expected Exception
        }

        // Set_InvalidProductID_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(-23, 0, 5, "Test")]
        [DataRow(-25, 5, 0, "Test")]
        [DataRow(0, 99, 0, "Test")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Set_InvalidProductID_ReturnError(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, loginHttpContext.Object);
            await service.Set(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====
            // Expected Exception
        }

        // Set_NoProductInDB_ReturnError // InvalidOperationException
        [DataTestMethod]
        [DataRow(23, 0, 5, "Test")]
        [DataRow(25, 5, 0, "Test")]
        [DataRow(99, 99, 0, "Test")]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Set_NoProductInDB_ReturnError(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await SetupUser(context, mapper, httpContext.Object, new UserRegisterDto { Password="test", Username = "test" });

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, loginHttpContext.Object);
            await service.Set(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====
            // Expected Exception
        }

        // Set_StockNumberProductIsInsufficient_ReturnError // ArithmeticException
        [DataTestMethod]
        [DataRow(1, 0, 100, "Test")]
        [DataRow(5, 0, 120, "Test")]
        [DataRow(9, 0, 200, "Test")]
        [ExpectedException(typeof(ArithmeticException))]
        public async Task Set_StockNumberProductIsInsufficient_ReturnError(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            // ===== Act =====
            var service = new StockServices(context, mapper, loginHttpContext.Object);
            await service.Set(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====
            // Expected Exception
        }

        // Set_ValidStockNumber_ReturnStockData
        [DataTestMethod]
        [DataRow(1, 0, 40, "Test")]
        [DataRow(5, 5, 0, "Test")]
        [DataRow(9, 0, 5, "Test")]
        public async Task Set_ValidStockNumber_ReturnStockData(int id, int debit, int credit, string remark)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Get product balance
            var dataDebit = await context.Stock
                                        .Where(_ => _.ProductId == id)
                                        .SumAsync(_ => _.Debit);


            var dataCredit = await context.Stock
                                         .Where(_ => _.ProductId == id)
                                         .SumAsync(_ => _.Credit);

            var balance = dataDebit - dataCredit;

            var count = await context.Stock.CountAsync();

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, loginHttpContext.Object);
            var result = await service.Set(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = remark });

            // ===== Assert =====

            // Check on result
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(balance, result.Data.StockBefore);
            Assert.AreEqual(balance + debit - credit, result.Data.StockAfter);

            // Check on database
            var assContext = BuildContext(dbName);
            var assCount = await assContext.Stock.CountAsync();
            var assStock = await assContext.Stock.LastAsync();

            Assert.AreEqual(count +1 , assCount);
            Assert.AreEqual(id, assStock.ProductId);
            Assert.AreEqual(debit, assStock.Debit);
            Assert.AreEqual(credit, assStock.Credit);
            Assert.AreEqual(balance, assStock.StockBefore);
            Assert.AreEqual(balance + debit - credit, assStock.StockAfter);
        }
        #endregion

        #region ProductIsSufficient
        // ProductIsSufficient_InvalidProductID_ReturnError // ArgumentOutOfRangeException
        [DataTestMethod]
        [DataRow(0, 0, 40)]
        [DataRow(-5, 5, 0)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task ProductIsSufficient_InvalidProductID_ReturnError(int id, int debit, int credit)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act =====
            var service = new StockServices(context, mapper, httpContext.Object);
            await service.ProductIsSufficient(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = "" });

            // ===== Assert =====
            // Expected Exception
        }

        // ProductIsSufficient_NoProductInDB_ReturnError // InvalidOperationException
        [DataTestMethod]
        [DataRow(1, 0, 40)]
        [DataRow(5, 5, 0)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ProductIsSufficient_NoProductInDB_ReturnError(int id, int debit, int credit)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // ===== Act =====
            var service = new StockServices(context, mapper, httpContext.Object);
            await service.ProductIsSufficient(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = "" });

            // ===== Assert =====
            // Expected Exception
        }

        // ProductIsSufficient_StockNumberProductIsInsufficient_ReturnError // ArithmeticException
        [DataTestMethod]
        [DataRow(1, 0, 200)]
        [DataRow(5, 0, 100)]
        [DataRow(7, 0, 10)]
        [ExpectedException(typeof(ArithmeticException))]
        public async Task ProductIsSufficient_StockNumberProductIsInsufficient_ReturnError(int id, int debit, int credit)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, httpContext.Object);
            await service.ProductIsSufficient(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = "" });

            // ===== Assert =====
            // Expected Exception
        }

        // ProductIsSufficient_ValidStockNumber_ReturnData
        [DataTestMethod]
        [DataRow(1, 0, 10)]
        [DataRow(5, 20, 0)]
        [DataRow(9, 20, 0)]
        public async Task ProductIsSufficient_ValidStockNumber_ReturnData(int id, int debit, int credit)
        {
            // ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var user = await Generate_Stock_Data(context, mapper, httpContext.Object);

            // Get product balance
            var dataDebit = await context.Stock
                                        .Where(_ => _.ProductId == id)
                                        .SumAsync(_ => _.Debit);


            var dataCredit = await context.Stock
                                         .Where(_ => _.ProductId == id)
                                         .SumAsync(_ => _.Credit);

            var balance = dataDebit - dataCredit;

            // ===== Act =====
            var actContext = BuildContext(dbName);
            var service = new StockServices(actContext, mapper, httpContext.Object);

            int resultBalance;
            decimal resultPrice;
            (resultBalance, resultPrice) = await service.ProductIsSufficient(id, new ProductStockAddDTO { Debit = debit, Credit = credit, Remark = "" });

            // ===== Assert =====
            Assert.AreEqual(balance, resultBalance);
        }

        #endregion
    }
}
