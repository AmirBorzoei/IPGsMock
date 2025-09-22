using System.ComponentModel.DataAnnotations;

namespace IPGsMock.Models;

public class ErrorResponse
{
    private ErrorResponse(string errorCode, string errorDesc)
    {
        ErrorCode = errorCode;
        ErrorDesc = errorDesc;
    }


    [Required]
    public int Status { get; init; } = -1;

    [Required]
    public string ErrorCode { get; init; }

    [Required]
    public string ErrorDesc { get; init; }


    public static ErrorResponse CreateErrorResponse(string errorCode, string errorDesc)
    {
        return new ErrorResponse(errorCode, errorDesc);
    }

    public static ErrorResponse CreateCanceledByUserResponse()
    {
        return CreateErrorResponse("1", "کاربر انصراف داده است.");
    }

    public static ErrorResponse CreateFailedResponse()
    {
        return CreateErrorResponse("3", "پرداخت انجام نشد.");
    }

    public static ErrorResponse CreateSessionIsNullResponse()
    {
        return CreateErrorResponse("4", "کاربر در بازه زمانی تعیین شده پاسخی ارسال نکرده است.");
    }

    public static ErrorResponse CreateInvalidParametersResponse()
    {
        return CreateErrorResponse("5", "پارامترهای ارسالی نامعتبر است.");
    }

    public static ErrorResponse CreateMerchantIpAddressIsInvalidResponse()
    {
        return CreateErrorResponse("8", "آدرس سرور پذیرنده نامعتبر است )در پرداخت های بر پایه توکن(");
    }

    public static ErrorResponse CreateTokenNotFoundResponse()
    {
        return CreateErrorResponse("10", "توکن ارسال شده یافت نشد.");
    }

    public static ErrorResponse CreateTokenRequiredResponse()
    {
        return CreateErrorResponse("11", "با این شماره ترمینال فقط تراکنش های توکنی قابل پرداخت هستند.");
    }

    public static ErrorResponse CreateTerminalNotFoundResponse()
    {
        return CreateErrorResponse("12", "شماره ترمینال ارسال شده یافت نشد.");
    }

    public static ErrorResponse CreateMultisettlePolicyErrorsResponse()
    {
        return CreateErrorResponse("21", "محدودیت های مدل چند حسابی رعایت نشده");
    }
}