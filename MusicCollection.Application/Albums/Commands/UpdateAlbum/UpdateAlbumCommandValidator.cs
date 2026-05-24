using FluentValidation;

namespace MusicCollection.Application.Albums.Commands.UpdateAlbum;

public class UpdateAlbumCommandValidator : AbstractValidator<UpdateAlbumCommand>
{
    public UpdateAlbumCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Некорректный идентификатор альбома.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название альбома не может быть пустым.")
            .MaximumLength(300).WithMessage("Название альбома не должно превышать 300 символов.");

        RuleFor(x => x.ReleaseYear)
            .NotEmpty().WithMessage("Год выпуска должен быть указан.")
            .GreaterThanOrEqualTo(1982).WithMessage("Год выпуска не может быть раньше 1982.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).WithMessage("Год выпуска не может быть из далекого будущего.");

        RuleFor(x => x.CatalogNumber)
            .MaximumLength(50).WithMessage("Каталожный номер не должен быть длиннее 50 символов.");

        RuleFor(x => x.Label)
            .MaximumLength(150).WithMessage("Название лейбла не должно превышать 150 символов.");

        RuleFor(x => x.Packaging)
            .IsInEnum().WithMessage("Указан недопустимый формат упаковки.");
    }
}
