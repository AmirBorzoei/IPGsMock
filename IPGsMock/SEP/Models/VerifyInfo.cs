namespace IPGsMock.SEP.Models;

public class VerifyInfo
{
    public string RRN { get; set; } = null!;
    public string RefNum { get; set; } = null!;
    public string MaskedPan { get; set; } = null!;
    public string HashedPan { get; set; } = null!;
    public long TerminalNumber { get; set; }
    public long OrginalAmount { get; set; }
    public long AffectiveAmount { get; set; }
    public string StraceDate { get; set; } = null!;
    public string StraceNo { get; set; } = null!;
}