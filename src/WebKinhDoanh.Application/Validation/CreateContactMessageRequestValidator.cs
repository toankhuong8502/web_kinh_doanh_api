using FluentValidation;
using WebKinhDoanh.Application.Dtos.Contacts;

namespace WebKinhDoanh.Application.Validation;

public sealed class CreateContactMessageRequestValidator : AbstractValidator<CreateContactMessageRequest>
{
    public CreateContactMessageRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(120);
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(request => request.Phone)
            .Matches(@"^[0-9+().\-\s]{9,20}$")
            .When(request => !string.IsNullOrWhiteSpace(request.Phone))
            .WithMessage("Số điện thoại không hợp lệ.");
        RuleFor(request => request.Subject).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Message).NotEmpty().MaximumLength(5000);
    }
}
