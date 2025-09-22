using IPGsMock.Models;
using Microsoft.AspNetCore.Mvc;

namespace IPGsMock.Controllers.SEP;

[Route("api/sep-mock")]
[ApiController]
public class SEPMockController : ControllerBase
{
    [HttpPost("onlinepg/onlinepg")]
    public IActionResult InitiatePaymentAsync([FromBody] InitiatePaymentRequest? request)
    {
        ErrorResponse? errorResponse = null;
        if (request is null)
        {
            errorResponse = ErrorResponse.CreateErrorResponse("5", "پارامترهای ارسالی نامعتبر است.");
            return Ok(errorResponse);
        }
        errorResponse = request.Validate();
        if (errorResponse != null)
        {
            return Ok(errorResponse);
        }

        var token = "2c3c1fefac5a48geb9f9be7e445dd9b2";
        var response = InitiatePaymentResponse.CreateSuccessResponse(token);

        return Ok(response);
    }
}