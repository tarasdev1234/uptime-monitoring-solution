using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Api.Middlewares {
    public class ApiExceptionsHandlerMiddleware {
        private readonly RequestDelegate next;

        public ApiExceptionsHandlerMiddleware (RequestDelegate next) {
            this.next = next;
        }

        public async Task Invoke (HttpContext context) {
            try {
                await next(context);
            } catch (Exception ex) {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync (HttpContext context, Exception exception) {
            var code = HttpStatusCode.InternalServerError;
            
            var result = JsonConvert.SerializeObject(new { error = GetErrorMessage(exception) });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }

        private static string GetErrorMessage(Exception e) {
            if (e.InnerException != null) {
                return GetErrorMessage(e.InnerException);
            }

            return e.Message;
        }
    }
}
