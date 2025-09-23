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

    [HttpGet("payment-gateway")]
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

        string state;
        bool isSuccessful;
        switch (actionType.ToLower())
        {
            case "success":
                state = "OK";
                isSuccessful = true;
                break;
            case "failure":
                state = "NOK";
                isSuccessful = false;
                break;
            // برای دکمه‌های اضافی، case اضافه کنید
            // case "cancel":
            //     state = "CanceledByUser";
            //     isSuccessful = false;
            //     break;
            default:
                return BadRequest("اکشن نامعتبر.");
        }

        var callbackForm = $@"
        <form id='callbackForm' action='{initiatePaymentRequest.RedirectUrl}' method='post'>
            <input type='hidden' name='State' value='{state}' />
            <input type='hidden' name='RefNum' value='{initiatePaymentRequest.RefNum}' />
            <input type='hidden' name='ResNum' value='{initiatePaymentRequest.ResNum}' />
            <input type='hidden' name='TraceNo' value='123456' />
        </form>
        <script>document.getElementById('callbackForm').submit();</script>";

        return Content(callbackForm, "text/html");
    }
}