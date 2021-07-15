using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmileShop.Exceptions;
using SmileShop.Models;
using System;
using System.Net;
using System.Net.Http;

namespace SmileShop.Controllers
{
    //[Route("/api")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/api/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var errorResult = ResponseResult.Failure<string>("Bad Request");

            if (context is null)
            {
                return BadRequest(errorResult);
            }

            var error = context.Error;

            var exType = error.GetType();

            if (exType.Name == "ApiException")
            {
                var response = ResponseResult.Failure<string>(error.Message);
                switch (error.Data["response"])
                {
                    case ResponseType.BadRequest:
                        return BadRequest(response);
                    case ResponseType.Conflict:
                        return Conflict(response);
                    case ResponseType.NoContent:
                        return NoContent();
                    case ResponseType.NotFound:
                        return NotFound(response);
                    case ResponseType.Unauthorized:
                        return Unauthorized(response);
                }

            }

            switch (exType.Name)
            {
                case "SqlException":
                    errorResult = ResponseResult.Failure<string>("DB Error");
                    var result = new ObjectResult(errorResult) { StatusCode = 500 };
                    return result;
                    //break;
                default:
                    errorResult = ResponseResult.Failure<string>("API Internal Error");
                    break;
            }

            //var error = Problem();
            //var errorStatus = error.StatusCode;
            //var errorMessage = context.Error.Message;

            return BadRequest(errorResult);
        }

        //[HttpPost]
        //public IActionResult Post() => RedirectToAction("Error");

        //[HttpPut]
        //public IActionResult Put() => RedirectToAction("Error");

        //[HttpDelete]
        //public IActionResult Delete() => RedirectToAction("Error");

        /*
        [Route("/api/error-development")]
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            //return Ok(webHostEnvironment.EnvironmentName);

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var errorResult = ResponseResult.Failure<string>("Bad Request");

            if (context is null)
            {
                return BadRequest(errorResult);
            }


            var error = context.Error;
            var exType = error.GetType();

            if (exType.Name == "ApiException")
            {
                var response = ResponseResult.Failure<string>(error.Message);
                switch (error.Data["response"])
                {
                    case ResponseType.BadRequest:
                        return BadRequest(response);
                    case ResponseType.Conflict:
                        return Conflict(response);
                    case ResponseType.NoContent:
                        return NoContent();
                    case ResponseType.NotFound:
                        return NotFound(response);
                    case ResponseType.Unauthorized:
                        return Unauthorized(response);
                }

            }

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }
        */
    }
}
