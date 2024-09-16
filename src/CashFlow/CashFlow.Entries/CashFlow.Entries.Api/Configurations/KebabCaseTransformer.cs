using System.Text.RegularExpressions;

namespace CashFlow.Entries.Api.Configurations
{
    public class KebabCaseTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) { return null; }
            string? str = value.ToString();
            if (string.IsNullOrEmpty(str)) { return null; }

            return Regex.Replace(str, "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }

}
