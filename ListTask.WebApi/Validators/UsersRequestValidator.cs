using FluentValidation;
using ListTask.Core.Const;
using ListTask.WebApi.Model;

namespace ListTask.WebApi.Validators;

public sealed class UsersRequestValidator : AbstractValidator<UsersRequest>
{
    public UsersRequestValidator()
    {
        RuleFor(x => x.Skip)
            .NotNull()
            .WithMessage(ListTaskConst.GeneralPaginationError);
        
        RuleFor(x => x.Take)
            .NotNull()
            .GreaterThan(0)
            .WithMessage(ListTaskConst.GeneralPaginationError);
    }
}