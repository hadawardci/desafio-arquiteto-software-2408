using CashFlow.Consolidated.Infra.Data.Contracts;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CashFlow.Consolidated.Infra.Data.DatabaseConfigurations
{
    public class DatabaseContext: IDatabaseContext
    {
        private readonly IMongoDatabase _database;

        public DatabaseContext(IConfiguration configuration, DatabaseConfigurationValue databaseConfiguration)
        {
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));

            var pack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true),
                new StringIdStoredAsObjectIdConvention(),
                new StringObjectIdIdGeneratorConvention(),
                new NamedIdMemberConvention("Id"),
                new IgnoreIfNullConvention(false),
                new NamedParameterCreatorMapConvention(),
                new MemberNameElementNameConvention(),
                new CamelCaseElementNameConvention(),
            };

            ConventionRegistry.Register(
               "Conventions",
               pack,
               t => t.Namespace.Contains("Domain"));

            var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var databaseName = databaseConfiguration.Name;
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoDatabase Database => _database;
    }
}
