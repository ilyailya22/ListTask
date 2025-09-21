using FluentValidation;
using ListTask.Core.Const;
using ListTask.WebApi.Model;

namespace ListTask.WebApi.Validators;

public sealed class CreateTaskListRequestValidator : AbstractValidator<CreateTaskListRequest>
{
    public CreateTaskListRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(ListTaskConst.StringMaxLength)
            .WithMessage(ListTaskConst.GeneralNameError);
        
        RuleFor(x => x.UserUniqueId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .WithMessage(ListTaskConst.GeneralUniqueIdError);
    }
}