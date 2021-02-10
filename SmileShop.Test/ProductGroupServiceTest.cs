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
        public async Task ProductGroupGetAll_NoData_ReturnErrorMessage()
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
            Assert.AreEqual(result.IsSuccess, false);
            Assert.AreEqual(result.Message, "No Product Group in this query");
        }

        // GetAll_HaveData_ReturnResultWithPagination
        // GetAll_CanDoPaginationOn2ndPage_ReturnResultWithPagination
        // GetAll_CanFilterByName_ReturnFilteredListOfProductGroup

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

        public List<ProductGroup> ProductGroupData()
        {
            List<ProductGroup> productGroups = new List<ProductGroup> {
                new ProductGroup { Id = 1, Name = "Test Product Group 1", CreatedByUserId = Guid.NewGuid(), CreatedDate = DateTime.Now },
                new ProductGroup { Id = 2, Name = "Test Product Group 2", CreatedByUserId = Guid.NewGuid(), CreatedDate = DateTime.Now },
                new ProductGroup { Id = 3, Name = "Test Product Group 3", CreatedByUserId = Guid.NewGuid(), CreatedDate = DateTime.Now },
                new ProductGroup { Id = 4, Name = "Test Product Group 4", CreatedByUserId = Guid.NewGuid(), CreatedDate = DateTime.Now },
                new ProductGroup { Id = 5, Name = "Test Product Group 5", CreatedByUserId = Guid.NewGuid(), CreatedDate = DateTime.Now },
            };

            return productGroups;
        
        }
    }
}
