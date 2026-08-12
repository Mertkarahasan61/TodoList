using FluentValidation;
using TodoList.Application.DTOs.Todos;
// Var olan görev değiştirilirken çalışır.
namespace TodoList.Application.Validators.Todos;

public class UpdateTodoRequestDtoValidator
    : AbstractValidator<UpdateTodoRequestDto>
{
    public UpdateTodoRequestDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Geçerli bir görev numarası gönderilmelidir.");    // Güncelleme DTO’sunda ayrıca Id bulunduğu için ekledik validator sadece sayının uygun olup olmadıgına bakar 
                                                                            // Görevin gerçekten bulunup bulunmadığını Service katmanında kontrol edeceğiz:
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Başlık alanı zorunludur.")
            .MinimumLength(3)
                .WithMessage("Başlık en az 3 karakter olmalıdır.")
            .MaximumLength(150)
                .WithMessage("Başlık en fazla 150 karakter olmalıdır.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Açıklama en fazla 1000 karakter olmalıdır.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Geçerli bir öncelik seçilmelidir.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Son teslim tarihi geçmiş bir tarih olamaz.")
            .When(x => x.DueDate.HasValue);
    }
}