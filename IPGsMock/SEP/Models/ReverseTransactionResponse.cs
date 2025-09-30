namespace IPGsMock.SEP.Models;

public class ReverseTransactionResponse
{
    public VerifyInfo TransactionDetail { get; set; } = null!;
    public int ResultCode { get; set; }
    public string ResultDescription { get; set; } = null!;
    public bool Success { get; set; }

    public static ReverseTransactionResponse CreateSuccessResponse(VerifyTransactionResponse verifyTransactionResponse)
    {
        return new ReverseTransactionResponse
        {
            TransactionDetail = verifyTransactionResponse.TransactionDetail,
            ResultCode = 0,
            ResultDescription = "عملیات با موفقیت انجام شد.",
            Success = true
        };
    }

    public static ReverseTransactionResponse CreateTransactionNotFoundResponse()
    {
        return new ReverseTransactionResponse
        {
            TransactionDetail = new VerifyInfo(),
            ResultCode = -2,
            ResultDescription = "تراکنش یافت نشد.",
            Success = false
        };
    }
}