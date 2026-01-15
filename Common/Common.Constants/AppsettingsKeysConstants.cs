namespace Common.Constants;

public static class AppsettingsKeysConstants
{
    #region Connection strings

    public const string DefaultDbConnectionString = "DefaultConnectionString";
    public const string ConnectionStringsDefaultDbConnectionString = "ConnectionStrings:DefaultConnectionString";

    public const string MongoDbConnectionString = "MongoDbConnectionString";
    public const string MongoDbDatabase = "MongoDb:Database";

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
