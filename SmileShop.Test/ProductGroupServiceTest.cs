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


    }
}
