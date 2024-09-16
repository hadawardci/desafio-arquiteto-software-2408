using Newtonsoft.Json.Serialization;

namespace CashFlow.Consolidated.Infra.Data.Helpers
{
    public static class StringHelper
    {
        public static string CamelCase<T>() =>
            typeof(T).Name[..1].ToLower() + typeof(T).Name[1..];

        private static readonly NamingStrategy _snakeCaseNamingStrategy =
        new CamelCaseNamingStrategy();


    }
}
