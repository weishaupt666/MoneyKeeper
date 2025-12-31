namespace MoneyKeeper.Integrations.Nbp.Models;

public class NbpApiResponse
{
    public string Code { get; set; } = string.Empty;
    public List<NbpRate> Rates { get; set; } = new();
}


public class NbpRate
{
    public string No { get; set; } = string.Empty;
    public string EffectiveDate { get; set; } = string.Empty;
    public decimal Mid { get; set; }
}
