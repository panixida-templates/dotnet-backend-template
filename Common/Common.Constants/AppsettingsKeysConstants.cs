namespace Common.Constants;

public static class AppsettingsKeysConstants
{
    #region Connection strings

    public const string ConnectionStringsSection = "ConnectionStrings";

    public const string PostgreSqlConnectionString = "PostgreSqlConnectionString";
    public const string ConnectionStringsPostgreSqlConnectionString = $"{ConnectionStringsSection}:{PostgreSqlConnectionString}";

    public const string MongoDbConnectionString = "MongoDbConnectionString";
    public const string ConnectionStringsMongoDbConnectionString = $"{ConnectionStringsSection}:{MongoDbConnectionString}";

    public const string DalSection = "Dal";
    public const string DalUseEf = $"{DalSection}:UseEf";
    public const string DalUseMongo = $"{DalSection}:UseMongo";

    #endregion

    #region HTTP clients

    public const string ApiBaseAddress = "HttpClients:Api:BaseAddress";

    #endregion

    public const string IndentityAuthority = "IndentityAuthority";
    public const string ServiceName = "ServiceName";
}
