using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;  
using Microsoft.AspNetCore.Mvc;  
using Microsoft.Net.Http.Headers; 
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;

namespace MarpajarosTPVAPI.Classes
{

    public class ResultClass: ActionResult
    {

        public static ActionResult WithContent<T>(T _content)
        {
            var err = new ErrorClass();
            err.HasError = false;
            err.Message = null;
            return new JsonResult(new ReturnClass()
            {
                content = _content,
                Error = err
            })
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static ActionResult WithError(string message)
        {
            var err = new ErrorClass();
            err.HasError = true;
            err.Message = message;
            return new JsonResult(new ReturnClass()
            {
                content = null,
                Error = err
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public static ActionResult NotAuthorized(string message)
        {
            var err = new ErrorClass();
            err.HasError = true;
            err.Message = message;
            return new JsonResult(new ReturnClass()
            {
                content = null,
                Error = err
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        public static ActionResult NotFound(string message)
        {
            var err = new ErrorClass();
            err.HasError = true;
            err.Message = message;
            return new JsonResult(new ReturnClass()
            {
                content = null,
                Error = err
            })
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        public static ActionResult WithFile(string filePath)
        {
            var data = System.IO.File.ReadAllBytes(filePath);
            const string DefaultContentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return new FileContentResult(data, contentType);
        }

        public static ActionResult Forbidden(string message)
        {
            var err = new ErrorClass();
            err.HasError = true;
            err.Message = message;
            return new JsonResult(new ReturnClass()
            {
                content = null,
                Error = err
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
