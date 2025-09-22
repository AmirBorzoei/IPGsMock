using IPGsMock.Models;
using System.ComponentModel.DataAnnotations;

namespace IPGsMock.Controllers.SEP;

public class InitiatePaymentRequest
{
    public string? Action { get; set; }

    public long? Amount { get; set; }

    public long? Wage { get; set; }

    public long? AffectiveAmount { get; set; }

    public string? TerminalId { get; set; }

    public string? ResNum { get; set; }

    public string? CellNumber { get; set; }

    public int? TokenExpiryInMin { get; set; }

    public string? RedirectUrl { get; set; }


    public ErrorResponse? Validate()
    {
        if (string.IsNullOrEmpty(Action))
        {
            return ErrorResponse.CreateErrorResponse("5", ".پارامترهای ارسال شده نامعتبر است.; اکشن الزامی است");
        }
        if (Amount is null || Amount < 1)
        {
            return ErrorResponse.CreateErrorResponse("5", ".پارامترهای ارسال شده نامعتبر است.; مبلغ الزامی است");
        }
        if (string.IsNullOrEmpty(TerminalId))
        {
            return ErrorResponse.CreateErrorResponse("5", ".پارامترهای ارسال شده نامعتبر است.; شماره ترمینال فروشنده الزامی است");
        }
        if (string.IsNullOrEmpty(RedirectUrl))
        {
            return ErrorResponse.CreateErrorResponse("5", ".پارامترهای ارسال شده نامعتبر است.; آدرس برگشت به سایت فروشنده الزامی است");
        }

        return null;
    }
}