using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using System;
using AutoMapper;
using SmileShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Moq;
using SmileShop.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmileShop.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmileShop.Test
{
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
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            // var fakeTenantId = "abcd";
            // context.Request.Headers["Tenant-ID"] = fakeTenantId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(http);

            var pagination = new PaginationDto();
            var filter = "";
            var order = new DataOrderDTO();

            // Act
            var service = new ProductGroupServices(context, mapper, mockHttpContextAccessor.Object);
            var result = await service.GetAll(pagination, filter, order);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Data);
            Assert.AreEqual(result.Message, "No Product Group in this query");
        }

        // GetAll_HaveData_ReturnResultWithPagination
        [TestMethod]
        public async Task GetAll_HaveData_ReturnResultWithPagination()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context1 = BuildContext(dbName);
            var mapper = BuildMap();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(http);

            // Add Mockup data
            await ProductGroupData(context1, mapper, mockHttpContextAccessor.Object);

            //Console.WriteLine("Original Data : " + await context1.ProductGroup.CountAsync());

            // Prepare instance for testing
            var context2 = BuildContext(dbName);


            //Console.WriteLine("Fetching Data : " + await context2.ProductGroup.CountAsync());


            PaginationDto pagination = new PaginationDto { RecordsPerPage = 2 };
            string filter = null;
            DataOrderDTO order = new DataOrderDTO();

            PaginationDto pagination2 = new PaginationDto { Page = 2, RecordsPerPage = 2 };

            PaginationDto pagination3 = new PaginationDto { Page = 5, RecordsPerPage = 2 };

            // Act
            var service = new ProductGroupServices(context2, mapper, mockHttpContextAccessor.Object);
            var result = await service.GetAll(pagination, filter, order);

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


            // Assert
            Assert.AreEqual(result.IsSuccess, true);
            Assert.AreEqual(result.Data[0].Id, 1);
            Assert.IsNotNull(result.CurrentPage);
            Assert.IsNotNull(result.PageIndex);

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

            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context1 = BuildContext(dbName);
            var mapper = BuildMap();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(http);

            // Add Mockup data
            await ProductGroupData(context1, mapper, mockHttpContextAccessor.Object);

            // Prepare Query
            var pagination = new PaginationDto();

            string filter = "Test Product Group 3";
            string filter2 = "G";
            string filter3 = Guid.NewGuid().ToString();

            var order = new DataOrderDTO();

            // New context
            var actContext = BuildContext(dbName);

            // Act

            var service = new ProductGroupServices(actContext, mapper, mockHttpContextAccessor.Object);

            var result = await service.GetAll(pagination, filter, order);
            var result2 = await service.GetAll(pagination, filter2, order);
            var result3 = await service.GetAll(pagination, filter3, order);


            // Assert

            // Result 1 : Exact filter
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data[0].Name, filter);
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
        // GetList_NoFilter_ReturnErrorMessage
        // GetList_HaveDataAndFilter_ReturnFilteredListOfProductGroup

        // Get_NoData_ReturnErrorMessage
        // Get_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        // Get_HaveDataButIdIsNotExist_ReturnErrorMessage
        // Get_HaveDataAndProductID_ReturnProductGroupWithSameId

        // Add_NoLoginUser_ReturnErrorMessage
        // Add_SentBlankProductGroupName_ReturnErrorMessage
        // Add_WithData_ReturnAddedResult

        // Edit_NoData_ReturnErrorMessage
        // Edit_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        // Edit_HaveDataButIdIsNotExist_ReturnErrorMessage
        // Edit_SentBlankProductGroupName_ReturnErrorMessage
        // Edit_WithData_ReturnEditedResult

        // Delete_NoData_ReturnErrorMessage
        // Delete_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        // Delete_HaveDataButIdIsNotExist_ReturnErrorMessage
        // Delete_WithID_ReturnDeletedResult

        public async Task ProductGroupData(AppDBContext context, IMapper mapper, IHttpContextAccessor http)
        {
            // Stand in User
            var auth = new AuthService(http, context, mapper, SmileShop.Program.Configuration);
            var userdto = new UserRegisterDto { Username = "test", Password = "test" };
            await auth.Register(userdto);
            User user = await context.Users.SingleOrDefaultAsync(x => x.Username.ToLower().Equals(userdto.Username.ToLower()));

            var userId = user.Id;

            // Stand in Product Group
            List<ProductGroup> productGroups = new List<ProductGroup> {
                new ProductGroup { Id = 1, Name = "Test Product Group 1", CreatedByUserId = userId, CreatedDate = DateTime.Now },
                new ProductGroup { Id = 2, Name = "Test Product Group 2", CreatedByUserId = userId, CreatedDate = DateTime.Now },
                new ProductGroup { Id = 3, Name = "Test Product Group 3", CreatedByUserId = userId, CreatedDate = DateTime.Now },
                new ProductGroup { Id = 4, Name = "Test Product Group 4", CreatedByUserId = userId, CreatedDate = DateTime.Now },
                new ProductGroup { Id = 5, Name = "Test Product Group 5", CreatedByUserId = userId, CreatedDate = DateTime.Now },
            };

            // Add Product Group
            context.ProductGroup.AddRange(productGroups);
            await context.SaveChangesAsync();
        }

    }
}
