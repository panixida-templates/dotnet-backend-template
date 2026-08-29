using Organization.Product.Module.Domain.RepositoryExamples.Users;

namespace Organization.Product.Module.Application.RepositoryExamples.Users.Delete;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.Id)
            .MustBeValidDomainValue(UserId.Create);
    }
}
