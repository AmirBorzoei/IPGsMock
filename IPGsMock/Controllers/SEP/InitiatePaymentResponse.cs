using System.ComponentModel.DataAnnotations;

namespace IPGsMock.Controllers.SEP;

public class InitiatePaymentResponse
{
    private InitiatePaymentResponse(int status, string token)
    {
        Status = status;
        Token = token;
    }


    [Required]
    public int Status { get; init; }

    [Required]
    public string Token { get; init; }


    public static InitiatePaymentResponse CreateSuccessResponse(string token)
    {
        return new InitiatePaymentResponse(1, token);
    }
}