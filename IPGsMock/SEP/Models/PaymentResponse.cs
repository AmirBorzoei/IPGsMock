namespace IPGsMock.SEP.Models;

public class PaymentResponse
{
    public string? MID { get; set; }
    public string? State { get; set; }
    public int Status { get; set; }
    public string? RRN { get; set; }
    public string? RefNum { get; set; }
    public string? ResNum { get; set; }
    public string? TerminalId { get; set; }
    public string? TraceNo { get; set; }
    public decimal Amount { get; set; }
    public decimal? Wage { get; set; }
    public string? SecurePan { get; set; }
    public string? HashedCardNumber { get; set; }

    public string? RedirectUrl { get; set; }
}