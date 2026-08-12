using FluentValidation;
using TodoList.Application.DTOs.Todos;
// Yeni görev oluşturulurken çalışır
namespace TodoList.Application.Validators.Todos;
   // KULLANICIDAN GELEN VERİNİN KURALLARA UYGUN OLUP OLMADIGINI KONTROL ETCEK  
public class CreateTodoRequestDtoValidator
    : AbstractValidator<CreateTodoRequestDto>
{
    public CreateTodoRequestDtoValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)   // İlk hata çıkınca aynı alanın kalan kontrollerini durdurur.
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