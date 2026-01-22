namespace Dto.Storage;

public sealed record FileContent(Stream Content, string ContentType, string FileName);
