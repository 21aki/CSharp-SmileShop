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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmileShop.Test.UnitTest
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
        [TestCategory("GetAll")]
        [ExpectedException(typeof(InvalidOperationException), "Product Group is not Exist")]
        public async Task GetAll_NoData_ReturnErrorMessage()
        {
            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var pagination = new PaginationDto();
            var filter = "";
            var order = new DataOrderDTO();

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.GetAll(pagination, filter, order);

            /// ==== Assert =====
            /// Expected Exception
        }

        // GetAll_HaveData_ReturnResultWithPagination
        [TestMethod]
        [TestCategory("GetAll")]
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
            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Prepare instance for testing
            var actContext = BuildContext(dbName);

            PaginationDto pagination = new PaginationDto { RecordsPerPage = 2 };
            PaginationDto pagination2 = new PaginationDto { Page = 2, RecordsPerPage = 2 };
            PaginationDto pagination3 = new PaginationDto { Page = 5, RecordsPerPage = 2 };

            string filter = null;
            DataOrderDTO order = new DataOrderDTO();

            /// ===== Act =====
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            var result1 = await service.GetAll(pagination, filter, order);
            var result2 = await service.GetAll(pagination2, filter, order);

            /// ===== Assert =====
            // Result 1 : Return data on 1st Page
            Assert.AreEqual(result1.IsSuccess, true);
            Assert.AreEqual(result1.Data[0].Id, 1);
            Assert.IsNotNull(result1.CurrentPage);
            Assert.IsNotNull(result1.PageIndex);

            // Result 2 : Return data on 2nd Page
            Assert.AreEqual(result2.IsSuccess, true);
            Assert.AreEqual(result2.Data[0].Id, 3);

        }

        // GetAll_ReachUnexistPage_ReturnResultWithPagination
        [TestMethod]
        [TestCategory("GetAll")]
        [ExpectedException(typeof(InvalidOperationException), "Product Group is not Exist")]
        public async Task GetAll_ReachUnexistPage_ReturnResultWithPagination()
        {
            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // Add Mockup data
            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Prepare instance for testing
            var actContext = BuildContext(dbName);

            PaginationDto pagination = new PaginationDto { Page = 5, RecordsPerPage = 2 };

            string filter = null;
            DataOrderDTO order = new DataOrderDTO();

            /// ===== Act =====
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            var result = await service.GetAll(pagination, filter, order);

            /// ===== Assert =====
            // Result : Check on Page Doesn't exist
            // Expected Exception
        }

        // GetAll_CanFilterByName_ReturnFilteredListOfProductGroup
        [TestMethod]
        [TestCategory("GetAll")]
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
            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Prepare Query
            var pagination = new PaginationDto();

            string filter1 = "Test Product Group 3";
            string filter2 = "G";

            var order = new DataOrderDTO();

            // New context
            var actContext = BuildContext(dbName);

            /// ===== Act =====

            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            var result = await service.GetAll(pagination, filter1, order);
            var result2 = await service.GetAll(pagination, filter2, order);

            /// ===== Assert =====

            // Result 1 : Exact filter
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data[0].Name, filter1);
            Assert.AreEqual(result.Data.Count, 1);

            // Result 2 : Like cause filter
            Assert.IsTrue(result2.IsSuccess);
            Assert.IsTrue(result2.Data[0].Name.Contains(filter2));
            Assert.IsTrue(result2.Data[result2.Data.Count - 1].Name.Contains(filter2));
            Assert.AreEqual(result2.Data.Count, 5);

        }

        // GetAll_FilterIsNotContainInDatabase_ReturnError
        [TestMethod]
        [TestCategory("GetAll")]
        [ExpectedException(typeof(InvalidOperationException), "Product Group is not Exist")]
        public async Task GetAll_FilterIsNotContainInDatabase_ReturnError()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            // Add Mockup data
            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            // Prepare Query
            var pagination = new PaginationDto();

            string filter = Guid.NewGuid().ToString();

            var order = new DataOrderDTO();

            // New context
            var actContext = BuildContext(dbName);

            /// ===== Act =====

            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);
            var result = await service.GetAll(pagination, filter, order);


            /// ===== Assert =====


            // Result : Filter that not contain in Data
            // Expected Exception
        }

        // GetList_NoData_ReturnErrorMessage
        [TestMethod]
        [TestCategory("GetList")]
        [ExpectedException(typeof(InvalidOperationException), "Product Group is not Exist")]
        public async Task GetList_NoData_ReturnError()
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


            // Act
            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.GetList("Group");

            // Assert

            // Filter is presented, but there is not have data.
            // Expected Exception
        }

        // GetList_HaveNoFilter_ReturnError
        [TestMethod]
        [TestCategory("GetList")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetList_HaveNoFilter_ReturnError()
        {

            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);


            // Act
            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            var result = await service.GetList("");

            // Assert

            // Filter is presented, but there is not have data.
            // Expected Exception
        }

        // GetList_HaveDataAndFilter_ReturnFilteredListOfProductGroup <- (Merge) GetList_NoFilter_ReturnErrorMessage
        [TestMethod]
        [TestCategory("GetList")]
        public async Task GetList_HaveDataAndFilter_ReturnFilteredListOfProductGroup()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            var actContext = BuildContext(dbName);

            var result1 = new ServiceResponse<List<ProductGroupDTO>>();
            var result2 = new ServiceResponse<List<ProductGroupDTO>>();
            var result3 = new ServiceResponse<List<ProductGroupDTO>>();

            bool expectEx1 = false;
            bool expectEx2 = false;
            bool expectEx3 = false;

            /// ===== Act =====
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            try
            {
                result1 = await service.GetList("Group");
            }
            catch (Exception)
            {
                expectEx1 = true;
                throw;
            }

            try
            {
                result2 = await service.GetList("Group 3");
            }
            catch (Exception)
            {
                expectEx2 = true;
                throw;
            }

            try
            {
                result3 = await service.GetList(Guid.NewGuid().ToString());
            }
            catch (InvalidOperationException)
            {
                expectEx3 = true;
            }
            catch (Exception)
            {
                expectEx3 = true;
                throw;
            }


            /// ===== Assert =====


            // Result 1 : Filter is presented, And serach for "Group"
            Assert.IsFalse(expectEx1);
            Assert.IsTrue(result1.IsSuccess);
            Assert.AreEqual(result1.Data.Count, 5);
            Assert.IsTrue(result1.Data[0].Name.Contains("Group"));
            Assert.IsTrue(result1.Data[2].Name.Contains("Group"));

            // Result 2 : Filter is presented, And serach for "Group 3"
            Assert.IsFalse(expectEx2);
            Assert.IsTrue(result2.IsSuccess);
            Assert.AreEqual(result2.Data.Count, 1);
            Assert.IsTrue(result2.Data[0].Name.Contains("Group 3"));

            // Result 3 : Filter is presented, And serach for Random value that not exist in the data
            Assert.IsTrue(expectEx3);
            Assert.IsNull(result3.Data);


        }
        [TestMethod]
        [TestCategory("GetList")]
        [ExpectedException(typeof(InvalidOperationException), "Product Group is not Exist")]
        public async Task GetList_FilterIsNotContainInDatabase_ReturnError()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            var actContext = BuildContext(dbName);

            // ===== Act =====
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            var result = await service.GetList(Guid.NewGuid().ToString());

            /// ===== Assert =====
            // Result : Used filter that not contain in database
            // Expected Exception

        }

        // Get_NoData_ReturnErrorMessage
        // + (merge) Get_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Get")]
        public async Task Get_ProductGroupValidation_ReturnError()
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

            bool expectEx1 = false;
            bool expectEx2 = false;
            bool expectEx3 = false;

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);

            try
            {
                await service.Get(productGroupId1);
            }
            catch (ArgumentOutOfRangeException)
            {
                expectEx1 = true;
            }

            try
            {
                await service.Get(productGroupId2);
            }
            catch (ArgumentOutOfRangeException)
            {
                expectEx2 = true;
            }

            try
            {
                await service.Get(productGroupId3);
            }
            catch (InvalidOperationException)
            {
                expectEx3 = true;
            }

            /// ==== Assert =====

            // Result 1 : Id (-5) must be greater than 0, then error
            Assert.IsTrue(expectEx1);

            // Result 2 : Id (0) must be greater than 0, then error
            Assert.IsTrue(expectEx2);

            // Result 3 : Id must be greater than 0 but no data in database, then error
            Assert.IsTrue(expectEx3);
        }

        // Get_HaveDataAndProductID_ReturnProductGroupWithSameId
        // + (merge) Get_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        // + (merge) Get_HaveDataButIdIsNotExist_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Get")]
        public async Task Get_HaveDataAndProductID_ReturnProductGroupWithSameId()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();

            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            var productGroupId1 = -5;
            var productGroupId2 = 1;
            var productGroupId3 = 9;
            var productGroupId4 = 4;

            bool expectEx1 = false;
            bool expectEx2 = false;
            bool expectEx3 = false;
            bool expectEx4 = false;

            ServiceResponse<ProductGroupDTO> result1 = new ServiceResponse<ProductGroupDTO>();
            ServiceResponse<ProductGroupDTO> result2 = new ServiceResponse<ProductGroupDTO>();
            ServiceResponse<ProductGroupDTO> result3 = new ServiceResponse<ProductGroupDTO>();
            ServiceResponse<ProductGroupDTO> result4 = new ServiceResponse<ProductGroupDTO>();

            var actContext = BuildContext(dbName);

            /// ===== Act =====
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            try
            {
                result1 = await service.Get(productGroupId1);
            }
            catch (ArgumentOutOfRangeException)
            {
                expectEx1 = true;
            }

            try
            {
                result2 = await service.Get(productGroupId2);
            }
            catch (Exception)
            {
                expectEx2 = true;
            }


            try
            {
                result3 = await service.Get(productGroupId3);
            }
            catch (InvalidOperationException)
            {
                expectEx3 = true;
            }

            try
            {
                result4 = await service.Get(productGroupId4);
            }
            catch (Exception)
            {
                expectEx4 = true;
            }

            /// ==== Assert =====

            // Result 1 : Id (-5) must be greater than 0, then error
            Assert.IsNull(result1.Data);
            Assert.IsTrue(expectEx1);

            // Result 2 :  Have a data & ProductID, It's must return ProductGroup With Same Id
            Assert.IsTrue(result2.IsSuccess);
            Assert.IsNotNull(result2.Data);
            Assert.AreEqual(result2.Data.Id, productGroupId2);
            Assert.IsFalse(expectEx2);

            // Result 3 : Have a data but Id is not exist, Return error message
            Assert.IsNull(result3.Data);
            Assert.IsTrue(expectEx3);

            // Result 4 :  Have a data & ProductID, It's must return ProductGroup With Same Id
            Assert.IsTrue(result4.IsSuccess);
            Assert.IsNotNull(result4.Data);
            Assert.AreEqual(result4.Data.Id, productGroupId4);
            Assert.IsFalse(expectEx4);
        }

        // Add_NoLoginUser_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Add")]
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

            bool expectEx = false;

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);

            try
            {
                var result = await service.Add(addProductGroup);
            }
            catch (UnauthorizedAccessException)
            {
                expectEx = true;
            }

            /// ===== Assert =====

            // Check that database has no new record
            var resultContext = BuildContext(dbName);
            var recordCount = await resultContext.ProductGroup.CountAsync();

            Assert.AreEqual(recordCount, 0);

            // Result : Return an error message
            Assert.IsTrue(expectEx);
        }

        // Add_SentBlankProductGroupName_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Add")]
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

            bool expectEx = false;

            /// ===== Act =====
            var service = new ProductGroupServices(context, mapper, httpContext.Object);

            try
            {
                var result = await service.Add(addProductGroup);
            }
            catch (ArgumentNullException)
            {
                expectEx = true;
            }

            /// ===== Assert =====

            // Check that database has no new record
            var resultContext = BuildContext(dbName);
            var recordCount = await resultContext.ProductGroup.CountAsync();

            Assert.AreEqual(recordCount, 0);

            // Result : Return an error message
            Assert.IsTrue(expectEx);
        }

        // Add_WithData_ReturnAddedResult
        [TestMethod]
        [TestCategory("Add")]
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
        [TestCategory("Edit")]
        [ExpectedException(typeof(ArgumentNullException))]
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
            // Except Exception
        }

        // Edit_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Edit")]
        public async Task Edit_ProductIdIsLessOrEqualZero_ReturnErrorMessage()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            var editProductGroup = new ProductGroupAddDTO { };

            bool expectEx1 = false;
            bool expectEx2 = false;

            /// ===== Act =====

            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            try
            {
                await service.Edit(0, editProductGroup);
            }
            catch (ArgumentOutOfRangeException)
            {
                expectEx1 = true;
            }

            try
            {
                await service.Edit(-10, editProductGroup);
            }
            catch (ArgumentOutOfRangeException)
            {
                expectEx2 = true;
            }

            /// ===== Assert =====

            // Result 1 : When Id = 0
            Assert.IsTrue(expectEx1);

            // Result 1 : When Id = -10
            Assert.IsTrue(expectEx2);
        }

        // Edit_NoData_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Edit")]
        [ExpectedException(typeof(InvalidOperationException))]
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
            // Expected Exception
        }

        // Edit_WithData_ReturnEditedResult
        // + (merge) Edit_HaveDataButIdIsNotExist_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Edit")]
        public async Task Edit_WithData_ReturnEditedResult()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            var productGroupId1 = 1;
            var productGroupId2 = 3;
            var productGroupId3 = 99;

            var editProductGroup1 = new ProductGroupAddDTO { Name = "Test Product Group Edit 1" };
            var editProductGroup2 = new ProductGroupAddDTO { Name = "Test Product Group Edit 2" };
            var editProductGroup3 = new ProductGroupAddDTO { Name = "Test Product Group Edit 3" };

            bool expectEx1 = false;
            bool expectEx2 = false;
            bool expectEx3 = false;

            var result1 = new ServiceResponse<ProductGroupDTO>();
            var result2 = new ServiceResponse<ProductGroupDTO>();
            var result3 = new ServiceResponse<ProductGroupDTO>();

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

            try
            {
                result1 = await service.Edit(productGroupId1, editProductGroup1);
            }
            catch (Exception)
            {
                expectEx1 = true;
                throw;
            }

            try
            {
                result2 = await service.Edit(productGroupId2, editProductGroup2);
            }
            catch (Exception)
            {
                expectEx2 = true;
                throw;
            }

            try
            {
                result3 = await service.Edit(productGroupId3, editProductGroup3);
            }
            catch (InvalidOperationException)
            {
                expectEx3 = true;
            }

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

            Assert.IsFalse(expectEx1);

            // Result 2 : Edit ProductGroup (ID 3) Must be changed
            Assert.IsTrue(result2.IsSuccess);
            Assert.AreEqual(result2.Message, $"Product Group ({editProductGroup2.Name}) have been edited successfully");

            Assert.IsNotNull(chkProductGroup2);

            Assert.AreEqual(result2.Data.Id, productGroupId2);
            Assert.AreEqual(result2.Data.Id, chkProductGroup2.Id);

            Assert.AreNotEqual(result2.Data.Name, dataProductGroup2.Name);
            Assert.AreEqual(result2.Data.Name, chkProductGroup2.Name);

            Assert.IsFalse(expectEx2);

            // Result 3 : Edit ProductGroup (ID 99) Must be return an error message
            Assert.IsNull(result3.Data);
            Assert.IsTrue(expectEx3);

            Assert.IsNull(dataProductGroup3);
            Assert.IsNull(chkProductGroup3);

        }

        // Delete_NoData_ReturnErrorMessage
        // +(merge) Delete_ProductIdIsLessOrEqualZero_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Delete")]
        public async Task Delete_NoData_ReturnErrorMessage()
        {

            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            int productGroupId1 = 4;
            int productGroupId2 = 0;
            int productGroupId3 = -21;

            bool expectEx1 = false;
            bool expectEx2 = false;
            bool expectEx3 = false;

            /// ===== Act =====
            var result1 = new ServiceResponse<ProductGroupDTO>();
            var result2 = new ServiceResponse<ProductGroupDTO>();
            var result3 = new ServiceResponse<ProductGroupDTO>();

            var service = new ProductGroupServices(context, mapper, httpContext.Object);
            try
            {
                result1 = await service.Delete(productGroupId1);
            }
            catch (InvalidOperationException)
            {
                expectEx1 = true;
            }

            try
            {
                result2 = await service.Delete(productGroupId2);
            }
            catch (ArgumentOutOfRangeException)
            {
                expectEx2 = true;
            }

            try
            {
                result3 = await service.Delete(productGroupId3);

            }
            catch (ArgumentOutOfRangeException)
            {
                expectEx3 = true;
            }

            /// ===== Assert =====

            // Result 1 : No data in database must return an error message
            Assert.IsNull(result1.Data);
            Assert.IsTrue(expectEx1);

            // Result 2 : if ID (0) not grater than 0,  return an error message
            Assert.IsNull(result2.Data);
            Assert.IsTrue(expectEx2);

            // Result 3 : if ID (-21) not grater than 0,  return an error message
            Assert.IsNull(result3.Data);
            Assert.IsTrue(expectEx3);
        }

        // Delete_WithID_ReturnDeletedResult
        // + (merge) Delete_HaveDataButIdIsNotExist_ReturnErrorMessage
        [TestMethod]
        [TestCategory("Delete")]
        public async Task Delete_WithID_ReturnDeletedResult()
        {


            /// ===== Arrange =====
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();


            var httpContext = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            httpContext.Setup(_ => _.HttpContext).Returns(http);

            await Generate_ProductGroup_Data(context, mapper, httpContext.Object);

            int productGroupId1 = 1;
            int productGroupId2 = 4;
            int productGroupId3 = 99;

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
            bool expectEx1 = false;
            bool expectEx2 = false;
            bool expectEx3 = false;

            var result1 = new ServiceResponse<ProductGroupDTO>();
            var result2 = new ServiceResponse<ProductGroupDTO>();
            var result3 = new ServiceResponse<ProductGroupDTO>();

            /// ===== Act =====

            var actContext = BuildContext(dbName);
            var service = new ProductGroupServices(actContext, mapper, httpContext.Object);

            try
            {
                result1 = await service.Delete(productGroupId1);
            }
            catch (Exception)
            {
                expectEx1 = true;
                throw;
            }

            try
            {
                result2 = await service.Delete(productGroupId2);
            }
            catch (Exception)
            {
                expectEx2 = true;
                throw;
            }

            try
            {
                result3 = await service.Delete(productGroupId3);
            }
            catch (InvalidOperationException)
            {
                expectEx3 = true;
            }
            catch (Exception)
            {
                throw;
            }


            /// ===== Assert =====

            var assContext = BuildContext(dbName);

            // Arrange data for comparison.
            var chkProductGroup1 = await assContext.ProductGroup
                                                   .Where(x => x.Id == productGroupId1)
                                                   .FirstOrDefaultAsync();
            var chkProductGroup2 = await assContext.ProductGroup
                                                   .Where(x => x.Id == productGroupId2)
                                                   .FirstOrDefaultAsync();
            var chkProductGroup3 = await assContext.ProductGroup
                                                   .Where(x => x.Id == productGroupId3)
                                                   .FirstOrDefaultAsync();

            // Result 1 : ProductGroup (Id 1) Must be delete and return with Response
            Assert.IsFalse(expectEx1);
            Assert.IsTrue(result1.IsSuccess);
            Assert.AreEqual(result1.Message, $"Product Group ({dataProductGroup1.Name}) have been deleted successfully");

            Assert.IsNotNull(dataProductGroup1);
            Assert.IsNull(chkProductGroup1);

            // Result 2 : ProductGroup (Id 4) Must be delete and return with Response
            Assert.IsFalse(expectEx2);
            Assert.IsTrue(result2.IsSuccess);
            Assert.AreEqual(result2.Message, $"Product Group ({dataProductGroup2.Name}) have been deleted successfully");

            Assert.IsNotNull(dataProductGroup2);
            Assert.IsNull(chkProductGroup2);


            // Result 3 : ProductGroup (Id 99) is not in the database, return with Error Message
            Assert.IsTrue(expectEx3);

            Assert.IsNull(dataProductGroup3);
            Assert.IsNull(chkProductGroup3);
        }


    }
}
