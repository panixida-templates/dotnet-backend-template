namespace Organization.Product.Module.Application.RepositoryExamples.Users.Write.Delete;

public sealed record DeleteUserCommand(Guid Id)
    : ICommand<Result>;
