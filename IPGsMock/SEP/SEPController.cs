using IPGsMock.SEP.Models;
using Microsoft.AspNetCore.Mvc;

namespace IPGsMock.SEP;

[Route("sep")]
[ApiController]
public class SEPController : Controller
{
    private readonly InitiatePaymentRequestStorage _initiatePaymentRequestStorage;

    public SEPController(InitiatePaymentRequestStorage initiatePaymentRequestStorage)
    {
        _initiatePaymentRequestStorage = initiatePaymentRequestStorage;
    }


    [HttpPost("onlinepg/onlinepg")]
    public IActionResult InitiatePaymentAsync([FromBody] InitiatePaymentRequest? request)
    {
        ErrorResponse? errorResponse = null;
        if (request is null)
        {
            errorResponse = ErrorResponse.CreateInvalidParametersResponse();
            return Ok(errorResponse);
        }
        errorResponse = request.Validate();
        if (errorResponse != null)
        {
            return Ok(errorResponse);
        }

        var token = Guid.NewGuid().ToString().Replace("-", string.Empty);

        _initiatePaymentRequestStorage.Add(token, request);

        var response = InitiatePaymentResponse.CreateSuccessResponse(token);

        return Ok(response);
    }

    [HttpGet("payment-gateway")]
    public IActionResult PaymentGateway([FromQuery] string token)
    {
        var initiatePaymentRequest = _initiatePaymentRequestStorage.TryGetValue(token);

        if (initiatePaymentRequest is null)
        {
            var errorResponse = ErrorResponse.CreateTokenNotFoundResponse();
            return Ok(errorResponse);
        }

        return View("SEP/Views/PaymentGateway.cshtml", initiatePaymentRequest);
    }
}