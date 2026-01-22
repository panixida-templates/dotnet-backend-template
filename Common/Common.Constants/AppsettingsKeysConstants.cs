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
    public const string DalUseS3 = $"{DalSection}:UseS3";

    #endregion

    #region AWS / S3

    public const string S3Section = "S3";

    public const string S3ServiceUrl = $"{S3Section}:ServiceUrl";
    public const string S3Region = $"{S3Section}:Region";
    public const string S3ForcePathStyle = $"{S3Section}:ForcePathStyle";

    public const string S3AccessKey = $"{S3Section}:AccessKey";
    public const string S3SecretKey = $"{S3Section}:SecretKey";

    public const string S3BucketName = $"{S3Section}:BucketName";
    public const string S3BasePrefix = $"{S3Section}:BasePrefix";
    public const string S3DefaultPresignTtl = $"{S3Section}:DefaultPresignTtl";

    #endregion

    #region HTTP clients

    public const string ApiBaseAddress = "HttpClients:Api:BaseAddress";

    #endregion

    public const string IndentityAuthority = "IndentityAuthority";
    public const string ServiceName = "ServiceName";
}
