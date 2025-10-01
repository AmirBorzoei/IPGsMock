namespace IPGsMock.SEP.Models;

public class PaymentResponse
{
    public string MID { get; set; } = null!;
    public string State { get; set; } = null!;
    public int Status { get; set; }
    public string RRN { get; set; } = null!;
    public string RefNum { get; set; } = null!;
    public string ResNum { get; set; } = null!;
    public string TerminalId { get; set; } = null!;
    public string TraceNo { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal Wage { get; set; }
    public decimal AffectiveAmount { get; set; }
    public string SecurePan { get; set; } = null!;
    public string HashedCardNumber { get; set; } = null!;

    public DateTimeOffset StraceDate { get; set; }
    public string PersianStringStraceDate => PersianDateConverter.ToPersianDateString(StraceDate);

    public string RedirectUrl { get; set; } = null!;
}