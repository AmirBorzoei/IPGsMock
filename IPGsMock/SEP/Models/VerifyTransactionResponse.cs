namespace IPGsMock.SEP.Models;

public class VerifyTransactionResponse
{
    public VerifyInfo TransactionDetail { get; set; } = null!;
    public int ResultCode { get; set; }
    public string ResultDescription { get; set; } = null!;
    public bool Success { get; set; }

    public static VerifyTransactionResponse CreateSuccessResponse(PaymentResponse paymentResponse)
    {
        return new VerifyTransactionResponse
        {
            TransactionDetail = new VerifyInfo
            {
                RRN = paymentResponse!.RRN,
                RefNum = paymentResponse.RefNum,
                MaskedPan = paymentResponse.SecurePan,
                HashedPan = paymentResponse.HashedCardNumber,
                TerminalNumber = long.TryParse(paymentResponse!.TerminalId, out var tn) ? tn : 123_456_789,
                OrginalAmount = (long)paymentResponse.Amount,
                AffectiveAmount = (long)paymentResponse.Amount,
                StraceDate = paymentResponse.PersianStringStraceDate,
                StraceNo = paymentResponse.TraceNo
            },
            ResultCode = 0,
            ResultDescription = "عملیات با موفقیت انجام شد.",
            Success = true
        };
    }

    public static VerifyTransactionResponse CreateTransactionNotFoundResponse()
    {
        return new VerifyTransactionResponse
        {
            TransactionDetail = new VerifyInfo(),
            ResultCode = -2,
            ResultDescription = "تراکنش یافت نشد.",
            Success = false
        };
    }
}