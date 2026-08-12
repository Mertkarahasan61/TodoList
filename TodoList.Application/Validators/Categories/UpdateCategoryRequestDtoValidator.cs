using FluentValidation;
using TodoList.Application.DTOs.Categories;

namespace TodoList.Application.Validators.Categories;

public class UpdateCategoryRequestDtoValidator
    : AbstractValidator<UpdateCategoryRequestDto>
{
    public UpdateCategoryRequestDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)  // 0dan buyuk olmalı
            .WithMessage("Geçerli bir kategori numarası gönderilmelidir.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Kategori adı boş olamaz.")
            .MaximumLength(100)
            .WithMessage("Kategori adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Color)
            .MaximumLength(20)
            .WithMessage("Kategori rengi en fazla 20 karakter olabilir.");
    }
}