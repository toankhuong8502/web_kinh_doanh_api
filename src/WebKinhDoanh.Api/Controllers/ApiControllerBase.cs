using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Common;

namespace WebKinhDoanh.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected OkObjectResult Success<T>(T data, string message = "Thành công") =>
        Ok(ApiResponse<T>.Ok(data, message));

    protected OkObjectResult SuccessMsg(string message = "Thành công") =>
        Ok(ApiResponse<object?>.Ok(null, message));
}
