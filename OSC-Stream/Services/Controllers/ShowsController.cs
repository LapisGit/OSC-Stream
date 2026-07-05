using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSC_Stream.Data;
using OSC_Stream.Models.Shows;

namespace OSC_Stream.Services.Controllers;

[ApiController, Route("api/shows")]
public class ShowsController
{
    [HttpGet("all")]
    public async Task<IResult> GetAllShows([FromServices] AppDbContext db)
    {
        var shows = await db.Shows
            .Include(s => s.Episodes)
            .ToListAsync();

        var result = shows.Select(s => new
        {
            id = s.Id,
            name = s.Name,
            creator = s.Creator,
            bannerImage = s.BannerImage,
            dateReleased = s.DateReleased,
            episodeCount = s.Episodes?.Count ?? 0
        });

        return Results.Ok(result);
    }

    [HttpGet("{showId}")]
    public async Task<IResult> GetShow(int showId, [FromServices] AppDbContext db)
    {
        var show = await db.Shows
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show is null)
            return Results.NotFound();

        return Results.Ok(new
        {
            id = show.Id,
            name = show.Name,
            creator = show.Creator,
            bannerImage = show.BannerImage,
            dateReleased = show.DateReleased,
            episodes = show.Episodes?.Select(e => new
            {
                id = e.Id,
                showId = e.ShowId,
                name = e.Name,
                videoId = e.VideoId,
                bannerImage = e.BannerImage,
                dateReleased = e.DateReleased
            })
        });
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IResult> CreateShow([FromBody] CreateShowRequest request, [FromServices] AppDbContext db)
    {
        Show show = new()
        {
            Name = request.Name,
            Creator = request.Creator,
            BannerImage = request.BannerImage,
            DateReleased = request.DateReleased
        };

        db.Shows.Add(show);
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            id = show.Id,
            name = show.Name,
            creator = show.Creator,
            bannerImage = show.BannerImage,
            dateReleased = show.DateReleased,
            episodes = Array.Empty<object>()
        });
    }

    [HttpPost("{showId}/episodes")]
    [Authorize(Roles = "admin")]
    public async Task<IResult> CreateEpisode(int showId, [FromBody] CreateEpisodeRequest request, [FromServices] AppDbContext db)
    {
        bool showExists = await db.Shows.AnyAsync(s => s.Id == showId);
        if (!showExists)
            return Results.NotFound(new { error = "Show not found." });

        Episode episode = new()
        {
            ShowId = showId,
            Name = request.Name,
            VideoId = request.VideoId,
            DateReleased = request.DateReleased
        };

        Show show = await db.Shows
            .Include(s => s.Episodes)
            .FirstAsync(s => s.Id == showId);

        show.Episodes.Add(episode);
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            id = episode.Id,
            showId = episode.ShowId,
            name = episode.Name,
            videoId = episode.VideoId,
            bannerImage = episode.BannerImage,
            dateReleased = episode.DateReleased
        });
    }
}