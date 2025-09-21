using FluentValidation;
using ListTask.Core.Const;
using ListTask.WebApi.Model;

namespace ListTask.WebApi.Validators;

public sealed class TaskListsByUserIdRequestValidator : AbstractValidator<TaskListsByUserIdRequest>
{
    public TaskListsByUserIdRequestValidator()
    {
        RuleFor(x => x.UserUniqueId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .WithMessage(ListTaskConst.GeneralUniqueIdError);

        RuleFor(x => x.Skip)
            .NotNull()
            .WithMessage(ListTaskConst.GeneralPaginationError);
        
        RuleFor(x => x.Take)
            .NotNull()
            .GreaterThan(0)
            .WithMessage(ListTaskConst.GeneralPaginationError);
    }
}