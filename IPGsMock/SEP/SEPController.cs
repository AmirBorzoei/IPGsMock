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
    private const string IPRCacheKeyPostfix = "IPR";
    private const string PRCacheKeyPostfix = "PR";
    private const string VTRCacheKeyPostfix = "VTR";

    private readonly ObjectCacheStorage _objectCacheStorage = objectCacheStorage;

    [HttpPost("onlinepg/onlinepg")]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(InitiatePaymentResponse), (int)HttpStatusCode.OK)]
    public IActionResult InitiatePayment([FromBody] InitiatePaymentRequest? request)
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

        _objectCacheStorage.Add(token + IPRCacheKeyPostfix, request);
        _objectCacheStorage.Add(request.ResNum! + IPRCacheKeyPostfix, request);

        var response = InitiatePaymentResponse.CreateSuccessResponse(token);

        return Ok(response);
    }

    [HttpGet("OnlinePG/SendToken")]
    public IActionResult ViewPaymentGateway([FromQuery] string token)
    {
        var initiatePaymentRequest = _objectCacheStorage.TryGetValue(token + IPRCacheKeyPostfix);

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

        var initiatePaymentRequest = (InitiatePaymentRequest?)_objectCacheStorage.TryGetValue(token + IPRCacheKeyPostfix);

        if (initiatePaymentRequest is null)
        {
            var errorResponse = ErrorResponse.CreateTokenNotFoundResponse();
            return Ok(errorResponse);
        }

        var securePan = Random.Shared.Next(100_000, 1_000_000) + "******" + Random.Shared.Next(1_000, 10_000).ToString();
        var hashedCardNumber = ComputeSha256Hash(securePan);

        var paymentResponse = new PaymentResponse
        {
            MID = initiatePaymentRequest.TerminalId!,
            // State = string.Empty,
            // Status = 0,
            // RRN = string.Empty,
            // RefNum = string.Empty,
            ResNum = initiatePaymentRequest.ResNum!,
            TerminalId = initiatePaymentRequest.TerminalId!,
            // TraceNo = string.Empty,
            Amount = initiatePaymentRequest.Amount!.Value,
            Wage = initiatePaymentRequest.Wage ?? 0,
            SecurePan = securePan,
            HashedCardNumber = hashedCardNumber,

            StraceDate = DateTimeOffset.Now,

            RedirectUrl = initiatePaymentRequest.RedirectUrl!
        };

        switch (actionType.ToLower())
        {
            case "success":
                paymentResponse.State = "OK";
                paymentResponse.Status = 2;
                paymentResponse.RefNum = Guid.NewGuid().ToString().Replace("-", string.Empty)[..5];
                paymentResponse.RRN = Guid.NewGuid().ToString().Replace("-", string.Empty)[..10];
                paymentResponse.TraceNo = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
                paymentResponse.StraceDate = DateTimeOffset.Now;

                _objectCacheStorage.Add(token + PRCacheKeyPostfix, paymentResponse);

                return View("SEP/Views/PaymentSuccessful.cshtml", paymentResponse);
            case "failure":
                paymentResponse.State = "Failed";
                paymentResponse.Status = 3;

                _objectCacheStorage.Add(token + PRCacheKeyPostfix, paymentResponse);

                return View("SEP/Views/PaymentFailure.cshtml", paymentResponse);
            default:
                return BadRequest("اکشن نامعتبر.");
        }
    }

    [HttpPost("verifyTxnRandomSessionkey/ipg/VerifyTransaction")]
    [ProducesResponseType(typeof(VerifyTransactionResponse), (int)HttpStatusCode.OK)]
    public IActionResult VerifyTransaction([FromBody] VerifyTransactionRequest request)
    {
        var paymentResponse = (PaymentResponse?)_objectCacheStorage.TryGetValue(request.RefNum! + PRCacheKeyPostfix);
        if (paymentResponse is null)
        {
            var errorResponse = VerifyTransactionResponse.CreateTransactionNotFoundResponse();
            return Ok(errorResponse);
        }

        var verifyTransactionResponse = VerifyTransactionResponse.CreateSuccessResponse(paymentResponse);

        _objectCacheStorage.Add(request.RefNum + VTRCacheKeyPostfix, verifyTransactionResponse);

        return Ok(verifyTransactionResponse);
    }

    [HttpPost("verifyTxnRandomSessionkey/ipg/ReverseTransaction")]
    [ProducesResponseType(typeof(ReverseTransactionResponse), (int)HttpStatusCode.OK)]
    public IActionResult ReverseTransaction([FromBody] ReverseTransactionRequest request)
    {
        var verifyTransactionResponse = (VerifyTransactionResponse?)_objectCacheStorage.TryGetValue(request.RefNum + VTRCacheKeyPostfix);
        if (verifyTransactionResponse is null)
        {
            var errorResponse = ReverseTransactionResponse.CreateTransactionNotFoundResponse();
            return Ok(errorResponse);
        }

        var reverseTransactionResponse = ReverseTransactionResponse.CreateSuccessResponse(verifyTransactionResponse);

        return Ok(reverseTransactionResponse);
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