using FluentValidation;
using ListTask.Core.Const;
using ListTask.WebApi.Model;

namespace ListTask.WebApi.Validators;

public sealed class DeleteTaskListRequestValidator : AbstractValidator<DeleteTaskListRequest>
{
    public DeleteTaskListRequestValidator()
    {
        RuleFor(x => x.UserUniqueId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .WithMessage(ListTaskConst.GeneralUniqueIdError);
        
        RuleFor(x => x.TaskListUniqueId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .WithMessage(ListTaskConst.GeneralUniqueIdError);
    }
}