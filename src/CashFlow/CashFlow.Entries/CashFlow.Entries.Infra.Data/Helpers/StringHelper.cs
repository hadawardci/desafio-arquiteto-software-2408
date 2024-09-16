using Newtonsoft.Json.Serialization;

namespace CashFlow.Entries.Infra.Data.Helpers
{
    public static class StringHelper
    {
        public static string CamelCase<T>() =>
            typeof(T).Name[..1].ToLower() + typeof(T).Name[1..];

        private readonly static NamingStrategy _snakeCaseNamingStrategy =
        new CamelCaseNamingStrategy();


    }
}
