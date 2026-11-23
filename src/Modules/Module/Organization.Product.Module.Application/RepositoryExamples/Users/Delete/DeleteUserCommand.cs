namespace Organization.Product.Module.Application.RepositoryExamples.Users.Delete;

public sealed record DeleteUserCommand(Guid Id)
    : ICommand<Result>;
