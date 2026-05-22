using FluentValidation;

namespace MusicCollection.Application.Albums.Commands.CreateAlbum;

public class CreateAlbumCommandValidator : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumCommandValidator()
    {
        // Правила для Альбома
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название альбома не может быть пустым.")
            .MaximumLength(300).WithMessage("Название альбома не должно превышать 300 символов.");

        RuleFor(x => x.ReleaseYear)
            .NotEmpty().WithMessage("Год выпуска должен быть указан.")
            .GreaterThanOrEqualTo(1982).WithMessage("Год выпуска не может быть раньше 1982 (год изобретения CD).")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).WithMessage("Год выпуска не может быть из далекого будущего.");

        RuleFor(x => x.ArtistId)
            .GreaterThan(0).WithMessage("Необходимо выбрать исполнителя.");

        RuleFor(x => x.CatalogNumber)
            .MaximumLength(50).WithMessage("Каталожный номер не должен быть длиннее 50 символов.");

        RuleFor(x => x.Label)
            .MaximumLength(150).WithMessage("Название лейбла не должно превышать 150 символов.");

        // Валидация вложенной коллекции дисков
        RuleFor(x => x.Discs)
            .NotEmpty().WithMessage("Альбом должен содержать хотя бы один физический диск.");

        RuleForEach(x => x.Discs)
            .SetValidator(new CreateDiscDtoValidator());
    }
}

// Внутренний валидатор для дисков
public class CreateDiscDtoValidator : AbstractValidator<CreateDiscDto>
{
    public CreateDiscDtoValidator()
    {
        RuleFor(x => x.DiscNumber)
            .GreaterThan(0).WithMessage("Номер диска должен быть больше нуля.");

        RuleFor(x => x.DiscName)
            .MaximumLength(200).WithMessage("Название диска не должно превышать 200 символов.");

        RuleFor(x => x.Tracks)
            .NotEmpty().WithMessage("Физический диск должен содержать хотя бы один музыкальный трек.");

        RuleForEach(x => x.Tracks)
            .SetValidator(new CreateTrackDtoValidator());
    }
}

// Внутренний валидатор для треков
public class CreateTrackDtoValidator : AbstractValidator<CreateTrackDto>
{
    public CreateTrackDtoValidator()
    {
        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Номер трека должен быть больше нуля.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название песни не может быть пустым.")
            .MaximumLength(300).WithMessage("Название песни не должно превышать 300 символов.");

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Длительность песни должна быть больше нуля.");
    }
}