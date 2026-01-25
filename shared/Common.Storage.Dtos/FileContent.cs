namespace Common.Storage.Dtos;

public sealed record FileContent(Stream Content, string ContentType, string FileName);