using System.Net;
using System.Security.Cryptography;
using System.Text;
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
    [ApiExplorerSettings(IgnoreApi = true)]
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

        var pan = Guid.NewGuid().ToString().Replace("-", string.Empty)[..16];
        var securePan = pan[..6] + "******" + pan[12..16];
        var hashedCardNumber = ComputeSha256Hash(pan);

        var paymentResponse = new PaymentResponse
        {
            MID = initiatePaymentRequest.TerminalId,
            // State = string.Empty,
            // Status = 0,
            // RRN = string.Empty,
            RefNum = initiatePaymentRequest.RefNum,
            ResNum = initiatePaymentRequest.ResNum,
            TerminalId = initiatePaymentRequest.TerminalId,
            // TraceNo = string.Empty,
            Amount = initiatePaymentRequest.Amount!.Value,
            Wage = initiatePaymentRequest.Wage!.Value,
            SecurePan = securePan,
            HashedCardNumber = hashedCardNumber,

            RedirectUrl = initiatePaymentRequest!.RedirectUrl
        };

        switch (actionType.ToLower())
        {
            case "success":
                paymentResponse.State = "OK";
                paymentResponse.Status = 2;
                paymentResponse.RRN = Guid.NewGuid().ToString().Replace("-", string.Empty)[..10];
                paymentResponse.TraceNo = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
                return View("SEP/Views/PaymentSuccessful.cshtml", paymentResponse);
            case "failure":
                paymentResponse.State = "Failed";
                paymentResponse.Status = 3;
                return View("SEP/Views/PaymentFailure.cshtml", paymentResponse);
            default:
                return BadRequest("اکشن نامعتبر.");
        }
    }

    private static string ComputeSha256Hash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

        var stringBuilder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            stringBuilder.Append(bytes[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }
}