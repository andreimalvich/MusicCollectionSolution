using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Application.Albums.Commands.CreateAlbum;
using MusicCollection.Application.Albums.Commands.DeleteAlbum;
using MusicCollection.Application.Albums.Commands.UpdateAlbum;
using MusicCollection.Application.Albums.Queries.GetAlbumDetails;
using MusicCollection.Application.Albums.Queries.GetAlbumsCarousel;
using MusicCollection.Application.Artists.Commands.DeleteArtist;
using MusicCollection.Application.Artists.Queries.GetArtistsList;
using MusicCollection.Application.Tracks.Commands.AddTracksBatch;

namespace MusicCollection.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Регистрация сценариев чтения (Queries)
        services.AddTransient<IGetAlbumsCarouselQuery, GetAlbumsCarouselQuery>();
        services.AddTransient<IGetAlbumDetailsQuery, GetAlbumDetailsQuery>();
        services.AddTransient<IGetArtistsListQuery, GetArtistsListQuery>();

        // Регистрация сценариев записи (Commands)
        services.AddTransient<ICreateAlbumCommandService, CreateAlbumCommandService>();
        services.AddTransient<IUpdateAlbumCommandService, UpdateAlbumCommandService>();
        services.AddTransient<IAddTracksBatchCommandService, AddTracksBatchCommandService>();
        services.AddTransient<IDeleteAlbumCommandService, DeleteAlbumCommandService>();
        services.AddTransient<IDeleteArtistCommandService, DeleteArtistCommandService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Добавить другме сервисы если нужно
        return services;
    }
}
