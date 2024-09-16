namespace CashFlow.Consolidated.Infra.Data.Configurations
{
    public record ApplicationUserValue(
        string Url,
        string TokenType,
        Dictionary<string, string> Data);

}
