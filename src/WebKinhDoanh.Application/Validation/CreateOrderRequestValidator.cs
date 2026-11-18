using FluentValidation;
using WebKinhDoanh.Application.Dtos.Orders;

namespace WebKinhDoanh.Application.Validation;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.CustomerName).NotEmpty().MaximumLength(120);
        RuleFor(request => request.Phone)
            .NotEmpty()
            .Matches(@"^[0-9+().\-\s]{9,20}$")
            .WithMessage("Số điện thoại không hợp lệ.");
        RuleFor(request => request.Email)
            .EmailAddress()
            .When(request => !string.IsNullOrWhiteSpace(request.Email));
        RuleFor(request => request.Address).NotEmpty().MaximumLength(300);
        RuleFor(request => request.Note).MaximumLength(1000);
        RuleFor(request => request.Items)
            .NotEmpty()
            .Must(items => items.Count <= 50)
            .WithMessage("Mỗi đơn hàng tối đa 50 dòng sản phẩm.");
        RuleForEach(request => request.Items).ChildRules(item =>
        {
            item.RuleFor(value => value.ProductId).GreaterThan(0);
            item.RuleFor(value => value.Quantity).InclusiveBetween(1, 100);
            item.RuleFor(value => value.Size).MaximumLength(10);
        });
    }
}
