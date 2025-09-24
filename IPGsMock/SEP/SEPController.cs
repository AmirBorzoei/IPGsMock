using System.Net;
using IPGsMock.SEP.Models;
using Microsoft.AspNetCore.Mvc;

namespace IPGsMock.SEP;

[Route("sep")]
[ApiController]
public class SEPController(ObjectCacheStorage objectCacheStorage) : Controller
{
    private readonly ObjectCacheStorage _objectCacheStorage = objectCacheStorage;

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

        _objectCacheStorage.Add(token, request);

        var response = InitiatePaymentResponse.CreateSuccessResponse(token);

        return Ok(response);
    }

    [HttpGet("OnlinePG/SendToken")]
    public IActionResult ViewPaymentGateway([FromQuery] string token)
    {
        var initiatePaymentRequest = _objectCacheStorage.TryGetValue(token);

        if (initiatePaymentRequest is null)
        {
            var errorResponse = ErrorResponse.CreateTokenNotFoundResponse();
            return Ok(errorResponse);
        }

        ViewBag.Token = token;
        return View("SEP/Views/PaymentGateway.cshtml", initiatePaymentRequest);
    }

    [HttpPost("payment-gateway")]
    public IActionResult SubmitPaymentGateway()
    {
        var token = Request.Form["token"].ToString();
        var actionType = Request.Form["actionType"].ToString();

        var initiatePaymentRequest = (InitiatePaymentRequest?)_objectCacheStorage.TryGetValue(token);

        if (initiatePaymentRequest is null)
        {
            var errorResponse = ErrorResponse.CreateTokenNotFoundResponse();
            return Ok(errorResponse);
        }

        switch (actionType.ToLower())
        {
            case "success":
                return View("SEP/Views/PaymentSuccessful.cshtml");
            case "failure":
                return View("SEP/Views/PaymentFailure.cshtml");
            default:
                return BadRequest("اکشن نامعتبر.");
        }
    }
}