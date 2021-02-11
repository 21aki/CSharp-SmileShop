using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using System;
using AutoMapper;
using SmileShop.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using SmileShop.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmileShop.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.Linq;

namespace SmileShop.Test
{
    /**
     * Test Name : ProductGroupServiceTest
     * Created by : AkiAkira
     * Version 1.0 : 10 Feb 2021.
     * 
     * Test on ProductGroupService
     */

    [TestClass]
    public class ProductGroupServiceTest : TestBase
    {

        /// <summary>
        /// If Product Group method GetAll has no data in database
        /// it must return suscess with error message
        /// </summary>
        [TestMethod]
        public async Task GetAll_NoData_ReturnErrorMessage()
        {
            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            // var fakeTenantId = "abcd";
            // context.Request.Headers["Tenant-ID"] = fakeTenantId;
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var pagination = new PaginationDto();
            var filter = "";
            var order = new DataOrderDTO();

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.GetAll(pagination, filter, order);

            /// ==== Assert =====
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Data);
            Assert.AreEqual(result.Message, "No Product Group in this query");
        }

        // GetAll_HaveData_ReturnResultWithPagination
        [TestMethod]
        public async Task GetAll_HaveData_ReturnResultWithPagination()
        {
            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // Add Mockup data
            await ProductGroupData(context, mapper, httpContext.Object);

            //Console.WriteLine("Original Data : " + await context1.ProductGroup.CountAsync());

            // Prepare instance for testing
            var actContext = BuildContext(dbName);

            //Console.WriteLine("Fetching Data : " + await context2.ProductGroup.CountAsync());

            PaginationDto pagination = new PaginationDto { RecordsPerPage = 2 };
            PaginationDto pagination2 = new PaginationDto { Page = 2, RecordsPerPage = 2 };
            PaginationDto pagination3 = new PaginationDto { Page = 5, RecordsPerPage = 2 };

            string filter = null;
            DataOrderDTO order = new DataOrderDTO();

            /// ===== Act =====
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            var result1 = await service.GetAll(pagination, filter, order);
            var result2 = await service.GetAll(pagination2, filter, order);
            var result3 = await service.GetAll(pagination3, filter, order);

            /* 
             * Console.WriteLine();
             * Console.WriteLine("Pagination : ");
             * Console.WriteLine(ClassToJsonString(pagination));
             * Console.WriteLine("Filter : ");
             * Console.WriteLine(filter);
             * Console.WriteLine("Order : ");
             * Console.WriteLine(ClassToJsonString(order));
             * Console.WriteLine();
             * 
             * Console.WriteLine("Result : ");
             * Console.WriteLine(ClassToJsonString(result));
             */


            /// ===== Assert =====
            Assert.AreEqual(result1.IsSuccess, true);
            Assert.AreEqual(result1.Data[0].Id, 1);
            Assert.IsNotNull(result1.CurrentPage);
            Assert.IsNotNull(result1.PageIndex);

            // Check on 2nd Page
            Assert.AreEqual(result2.IsSuccess, true);
            Assert.AreEqual(result2.Data[0].Id, 3);

            // Check on Page Doesn't exist
            Assert.AreEqual(result3.IsSuccess, false);
            Assert.IsNull(result3.Data);
            Assert.AreEqual(result3.Message, "No Product Group in this query");
        }

        // GetAll_CanFilterByName_ReturnFilteredListOfProductGroup
        [TestMethod]
        public async Task GetAll_CanFilterByName_ReturnFilteredListOfProductGroup()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // Add Mockup data
            await ProductGroupData(context, mapper, httpContext.Object);

            // Prepare Query
            var pagination = new PaginationDto();

            string filter1 = "Test Product Group 3";
            string filter2 = "G";
            string filter3 = Guid.NewGuid().ToString();

            var order = new DataOrderDTO();

            // New context
            var actContext = BuildContext(dbName);

            /// ===== Act =====

            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            var result = await service.GetAll(pagination, filter1, order);
            var result2 = await service.GetAll(pagination, filter2, order);
            var result3 = await service.GetAll(pagination, filter3, order);


            /// ===== Assert =====

            // Result 1 : Exact filter
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data[0].Name, filter1);
            Assert.AreEqual(result.Data.Count, 1);

            // Result 2 : Like cause filter
            Assert.IsTrue(result2.IsSuccess);
            Assert.IsTrue(result2.Data[0].Name.Contains(filter2));
            Assert.IsTrue(result2.Data[result2.Data.Count-1].Name.Contains(filter2));
            Assert.AreEqual(result2.Data.Count, 5);

            // Result 3 : Filter that not contain in Data
            Assert.IsFalse(result3.IsSuccess);
            Assert.IsNull(result3.Data);
            Assert.AreEqual(result3.Message, "No Product Group in this query");
        }

        // GetList_NoData_ReturnErrorMessage
        [TestMethod]
        public async Task GetList_NoData_ReturnErrorMessage()
        {

            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            // var fakeTenantId = "abcd";
            // context.Request.Headers["Tenant-ID"] = fakeTenantId;
            httpContext.Setup(_ => _.HttpContext).Returns(http);
            
            var filter = "";


            // Act
            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result1 = await service.GetList("");
            var result2 = await service.GetList("Group");

            // Assert

            // Result 1 : No filter
            Assert.IsFalse(result1.IsSuccess);
            Assert.IsNull(result1.Data);
            Assert.AreEqual(result1.Message, "This query must provided filter");


            // Result 2 : Filter is presented, but there is not have data.
            Assert.IsFalse(result2.IsSuccess);
            Assert.IsNull(result2.Data);
            Assert.AreEqual(result2.Message, "No Product Group in this query");
        }

        // GetList_HaveDataAndFilter_ReturnFilteredListOfProductGroup <- (Merge) GetList_NoFilter_ReturnErrorMessage
        [TestMethod]
        public async Task GetList_HaveDataAndFilter_ReturnFilteredListOfProductGroup()
        {

            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await ProductGroupData(context, mapper, httpContext.Object);

            var actContext = BuildContext(dbName);

            // Act
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);
            var result1 = await service.GetList("");
            var result2 = await service.GetList("Group");
            var result3 = await service.GetList("Group 3");
            var result4 = await service.GetList(Guid.NewGuid().ToString());

            // Assert

            // Result 1 : No filter
            Assert.IsFalse(result1.IsSuccess);
            Assert.IsNull(result1.Data);
            Assert.AreEqual(result1.Message, "This query must provided filter");


            // Result 2 : Filter is presented, And serach for "Group"
            Assert.IsTrue(result2.IsSuccess);
            Assert.AreEqual(result2.Data.Count, 5);
            Assert.IsTrue(result2.Data[0].Name.Contains("Group"));
            Assert.IsTrue(result2.Data[2].Name.Contains("Group"));

            // Result 3 : Filter is presented, And serach for "Group 3"
            Assert.IsTrue(result3.IsSuccess);
            Assert.AreEqual(result3.Data.Count, 1);
            Assert.IsTrue(result3.Data[0].Name.Contains("Group 3"));

            // Result 4 : Filter is presented, And serach for Random value that not exist in the data
            Assert.IsFalse(result4.IsSuccess);
            Assert.IsNull(result4.Data);
            Assert.AreEqual(result4.Message, "No Product Group in this query");

        }

        // Get_NoData_ReturnErrorMessage
        // + (merge) Get_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        [TestMethod]
        public async Task Get_NoData_ReturnErrorMessage()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var productGroupId1 = -5;
            var productGroupId2 = 0;
            var productGroupId3 = 1;

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);

            var result1 = await service.Get(productGroupId1);
            var result2 = await service.Get(productGroupId2);
            var result3 = await service.Get(productGroupId3);

            /// ==== Assert =====

            // Result 1 : Id (-5) must be greater than 0, then error
            Assert.IsFalse(result1.IsSuccess);
            Assert.AreEqual(result1.Message, "Id must be greater than 0");


            // Result 1 : Id (0) must be greater than 0, then error
            Assert.IsFalse(result2.IsSuccess);
            Assert.AreEqual(result2.Message, "Id must be greater than 0");

            // Result 3 : Id must be greater than 0, then error
            Assert.IsFalse(result3.IsSuccess);
            Assert.AreEqual(result3.Message, "No Product Group in this query");
        }

        // Get_HaveDataAndProductID_ReturnProductGroupWithSameId
        // + (merge) Get_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        // + (merge) Get_HaveDataButIdIsNotExist_ReturnErrorMessage
        [TestMethod]
        public async Task Get_HaveDataAndProductID_ReturnProductGroupWithSameId()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await ProductGroupData(context, mapper, httpContext.Object);

            var productGroupId1 = -5;
            var productGroupId2 = 1;
            var productGroupId3 = 9;


            var actContext = BuildContext(dbName);

            /// ===== Act =====
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            var result1 = await service.Get(productGroupId1);
            var result2 = await service.Get(productGroupId2);
            var result3 = await service.Get(productGroupId3);

            /// ==== Assert =====

            // Result 1 : Id (-5) must be greater than 0, then error
            Assert.IsFalse(result1.IsSuccess);
            Assert.AreEqual(result1.Message, "Id must be greater than 0");


            // Result 1 :  Have a data & ProductID, It's must return ProductGroup With Same Id
            Assert.IsTrue(result2.IsSuccess);
            Assert.AreEqual(result2.Data.Id, productGroupId2);

            // Result 3 : Have a data but Id is not exist, Return error message
            Assert.IsFalse(result3.IsSuccess);
            Assert.AreEqual(result3.Message, "No Product Group in this query");
        }

        // Add_NoLoginUser_ReturnErrorMessage
        [TestMethod]
        public async Task Add_NoLoginUser_ReturnErrorMessage()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var addProductGroup = new ProductGroupAddDTO { Name = "Test" };

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.Add(addProductGroup);

            /// ===== Assert =====
            
            // Check that database has no new record
            var resultContext = BuildContext(dbName);
            var recordCount = await resultContext.ProductGroup.CountAsync();

            Assert.AreEqual(recordCount, 0);

            // Result : Return an error message
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.Message, "User must be presented to perform this method");
        }

        // Add_SentBlankProductGroupName_ReturnErrorMessage

        [TestMethod]
        public async Task Add_SentBlankProductGroupName_ReturnErrorMessage()
        {
            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var addProductGroup = new ProductGroupAddDTO { Name = "" };

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.Add(addProductGroup);


            /// ===== Assert =====

            // Check that database has no new record
            var resultContext = BuildContext(dbName);
            var recordCount = await resultContext.ProductGroup.CountAsync();

            Assert.AreEqual(recordCount, 0);

            // Result : Return an error message
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.Message, "Please fill Product Group's Name");
        }

        // Add_WithData_ReturnAddedResult
        [TestMethod]
        public async Task Add_WithData_ReturnAddedResult()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var addProductGroup = new ProductGroupAddDTO { Name = "Test" };

            var user = await SetupUser(context, mapper, httpContext.Object, new UserRegisterDto { Username = "test", Password = "test" });

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var loginHttpContext = new Mock<IHttpContextAccessor>();
            var loginHttp = new DefaultHttpContext();
            loginHttp.User.AddIdentity(new ClaimsIdentity(claims));
            loginHttpContext.Setup(_ => _.HttpContext).Returns(loginHttp);


            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, loginHttpContext.Object);
            var result = await service.Add(addProductGroup);

            /// ===== Assert =====

            // Check that database has new record
            var resultContext = BuildContext(dbName);
            var recordCount = await resultContext.ProductGroup.CountAsync();

            Assert.AreEqual(recordCount, 1);

            // Result : Return an error message
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Message, $"Product Group ({addProductGroup.Name}) have been added successfully");
        }

        // Edit_SentBlankProductGroupName_ReturnErrorMessage
        [TestMethod]
        public async Task Edit_SentBlankProductGroupName_ReturnErrorMessage()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var productGroupId = 1;
            var editProductGroup = new ProductGroupAddDTO { };

            /// ===== Act =====

            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.Edit(productGroupId, editProductGroup);

            /// ===== Assert =====

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.Message, "Please fill Product Group's Name");
        }

        // Edit_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        [TestMethod]
        public async Task Edit_ProductIdIsLessOrEqualZero_ReturnErrorMessage()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);
            
            var editProductGroup = new ProductGroupAddDTO {};

            /// ===== Act =====

            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result1 = await service.Edit(0, editProductGroup);
            var result2 = await service.Edit(-10, editProductGroup);

            /// ===== Assert =====

            // Result 1 : When Id = 0
            Assert.IsFalse(result1.IsSuccess);
            Assert.AreEqual(result1.Message, "Id must be greater than 0");

            // Result 1 : When Id = -10
            Assert.IsFalse(result2.IsSuccess);
            Assert.AreEqual(result2.Message, "Id must be greater than 0");
        }

        // Edit_NoData_ReturnErrorMessage
        [TestMethod]
        public async Task Edit_NoData_ReturnErrorMessage()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var editProductGroup = new ProductGroupAddDTO { Name = "Test Product Group" };

            /// ===== Act =====

            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.Edit(1, editProductGroup);

            /// ===== Assert =====

            // No data in database must return an error message
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.Message, "No Product Group in this query");
        }

        // Edit_WithData_ReturnEditedResult
        // + (merge) Edit_HaveDataButIdIsNotExist_ReturnErrorMessage
        [TestMethod]
        public async Task Edit_WithData_ReturnEditedResult()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await ProductGroupData(context, mapper, httpContext.Object);

            var productGroupId1 = 1;
            var productGroupId2 = 3;
            var productGroupId3 = 99;

            var editProductGroup1 = new ProductGroupAddDTO { Name = "Test Product Group Edit 1" };
            var editProductGroup2 = new ProductGroupAddDTO { Name = "Test Product Group Edit 2" };
            var editProductGroup3 = new ProductGroupAddDTO { Name = "Test Product Group Edit 3" };

            // Arrange data for later comparison.
            var dataProductGroup1 = await context.ProductGroup
                                                 .Where(x => x.Id == productGroupId1)
                                                 .FirstOrDefaultAsync();
            var dataProductGroup2 = await context.ProductGroup
                                                 .Where(x => x.Id == productGroupId2)
                                                 .FirstOrDefaultAsync();
            var dataProductGroup3 = await context.ProductGroup
                                                 .Where(x => x.Id == productGroupId3)
                                                 .FirstOrDefaultAsync();

            /// ===== Act =====

            var actContext = BuildContext(dbName);

            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);
            var result1 = await service.Edit(productGroupId1, editProductGroup1);
            var result2 = await service.Edit(productGroupId2, editProductGroup2);
            var result3 = await service.Edit(productGroupId3, editProductGroup3);

            /// ===== Assert =====

            var assContext = BuildContext(dbName);

            var chkProductGroup1 = await assContext.ProductGroup
                                                   .Where(x => x.Id == productGroupId1)
                                                   .FirstOrDefaultAsync();
            var chkProductGroup2 = await assContext.ProductGroup
                                                   .Where(x => x.Id == productGroupId2)
                                                   .FirstOrDefaultAsync();
            var chkProductGroup3 = await assContext.ProductGroup
                                                   .Where(x => x.Id == productGroupId3)
                                                   .FirstOrDefaultAsync();


            // Result 1 : Edit ProductGroup (ID 1) Must be changed
            Assert.IsTrue(result1.IsSuccess);
            Assert.AreEqual(result1.Message, $"Product Group ({editProductGroup1.Name}) have been edited successfully");

            Assert.IsNotNull(chkProductGroup1);

            Assert.AreEqual(result1.Data.Id, productGroupId1);
            Assert.AreEqual(result1.Data.Id, chkProductGroup1.Id);

            Assert.AreNotEqual(result1.Data.Name, dataProductGroup1.Name);
            Assert.AreEqual(result1.Data.Name, chkProductGroup1.Name);

            // Result 2 : Edit ProductGroup (ID 3) Must be changed
            Assert.IsTrue(result2.IsSuccess);
            Assert.AreEqual(result2.Message, $"Product Group ({editProductGroup2.Name}) have been edited successfully");

            Assert.IsNotNull(chkProductGroup2);

            Assert.AreEqual(result2.Data.Id, productGroupId2);
            Assert.AreEqual(result2.Data.Id, chkProductGroup2.Id);

            Assert.AreNotEqual(result2.Data.Name, dataProductGroup2.Name);
            Assert.AreEqual(result2.Data.Name, chkProductGroup2.Name);


            // Result 3 : Edit ProductGroup (ID 99) Must be return an error message
            Assert.IsFalse(result3.IsSuccess);
            Assert.AreEqual(result3.Message, "No Product Group in this query");

            Assert.IsNull(dataProductGroup3);
            Assert.IsNull(chkProductGroup3);

        }

        // Delete_NoData_ReturnErrorMessage
        // Delete_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        // Delete_HaveDataButIdIsNotExist_ReturnErrorMessage
        // Delete_WithID_ReturnDeletedResult

        public async Task<User> SetupUser(AppDBContext context, IMapper mapper, IHttpContextAccessor http, UserRegisterDto userdto)
        {
            // Stand in User
            var auth = new AuthService(http, context, mapper, SmileShop.Program.Configuration);
            await auth.Register(userdto);
            User user = await context.Users.SingleOrDefaultAsync(x => x.Username.ToLower().Equals(userdto.Username.ToLower()));

            return user;
        }
        public async Task ProductGroupData(AppDBContext context, IMapper mapper, IHttpContextAccessor http)
        {
            var user = await SetupUser(context, mapper, http, new UserRegisterDto { Username = "Test", Password = "Test" });

            // Stand in Product Group
            List<ProductGroup> productGroups = new List<ProductGroup> {
                new ProductGroup { Name = "Test Product Group 1", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 2", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 3", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 4", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 5", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
            };

            // Add Product Group
            context.ProductGroup.AddRange(productGroups);
            await context.SaveChangesAsync();
        }

    }
}
