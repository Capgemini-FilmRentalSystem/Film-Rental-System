using AutoMapper;
using FilmRentalStore.API.DTOs.Actor;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class ActorServiceTests
{
    private readonly Mock<IActorRepository> _actorRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ActorService _service;

    public ActorServiceTests()
    {
        _service = new ActorService(_actorRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAllActorsAsync_WhenActorsExist_ReturnsMappedActors()
    {
        var actors = new List<Actor> { new() { ActorId = 1, FirstName = "Tom", LastName = "Hanks" } };
        var response = new List<ActorResponseDto> { new() { ActorId = 1, FirstName = "Tom", LastName = "Hanks" } };

        _actorRepository.Setup(r => r.GetAllActorsAsync()).ReturnsAsync(actors);
        _mapper.Setup(m => m.Map<IEnumerable<ActorResponseDto>>(actors)).Returns(response);

        var result = await _service.GetAllActorsAsync();

        Assert.Single(result);
        Assert.Equal(1, result.First().ActorId);
    }

    [Fact]
    public async Task GetActorByIdAsync_WhenActorExists_ReturnsMappedActor()
    {
        var actor = new Actor { ActorId = 2, FirstName = "Brad", LastName = "Pitt" };
        var response = new ActorResponseDto { ActorId = 2, FirstName = "Brad", LastName = "Pitt" };

        _actorRepository.Setup(r => r.GetActorByIdAsync(2)).ReturnsAsync(actor);
        _mapper.Setup(m => m.Map<ActorResponseDto>(actor)).Returns(response);

        var result = await _service.GetActorByIdAsync(2);

        Assert.Equal(2, result.ActorId);
    }

    [Fact]
    public async Task CreateActorAsync_WhenValidDto_AddsActorAndReturnsCreatedActor()
    {
        var dto = new ActorRequestDto { FirstName = "Keanu", LastName = "Reeves" };
        var actor = new Actor { ActorId = 3, FirstName = "Keanu", LastName = "Reeves" };
        var response = new ActorResponseDto { ActorId = 3, FirstName = "Keanu", LastName = "Reeves" };

        _mapper.Setup(m => m.Map<Actor>(dto)).Returns(actor);
        _actorRepository.Setup(r => r.CreateActorAsync(actor)).Returns(Task.CompletedTask);
        _actorRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _actorRepository.Setup(r => r.GetActorByIdAsync(3)).ReturnsAsync(actor);
        _mapper.Setup(m => m.Map<ActorResponseDto>(actor)).Returns(response);

        var result = await _service.CreateActorAsync(dto);

        Assert.Equal(3, result.ActorId);
        _actorRepository.Verify(r => r.CreateActorAsync(actor), Times.Once);
        _actorRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateActorAsync_WhenValidDto_UpdatesActorAndReturnsUpdatedActor()
    {
        var dto = new ActorRequestDto { FirstName = "Updated", LastName = "Actor" };
        var actor = new Actor { ActorId = 4, FirstName = "Old", LastName = "Actor" };
        var response = new ActorResponseDto { ActorId = 4, FirstName = "Updated", LastName = "Actor" };

        _actorRepository.Setup(r => r.GetActorByIdAsync(4)).ReturnsAsync(actor);
        _mapper.Setup(m => m.Map<ActorRequestDto, Actor>(dto, actor)).Returns(actor);
        _actorRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<ActorResponseDto>(actor)).Returns(response);

        var result = await _service.UpdateActorAsync(4, dto);

        Assert.Equal(4, result.ActorId);
        _actorRepository.Verify(r => r.Update(actor), Times.Once);
        _actorRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllActorsAsync_WhenRepositoryReturnsEmpty_ThrowsNotFoundException()
    {
        _actorRepository.Setup(r => r.GetAllActorsAsync()).ReturnsAsync(new List<Actor>());

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllActorsAsync());
    }

    [Fact]
    public async Task GetActorByIdAsync_WhenActorDoesNotExist_ThrowsNotFoundException()
    {
        _actorRepository.Setup(r => r.GetActorByIdAsync(99)).ReturnsAsync((Actor?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetActorByIdAsync(99));
    }

    [Fact]
    public async Task CreateActorAsync_WhenDtoIsNull_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateActorAsync(null!));
    }

    [Fact]
    public async Task UpdateActorAsync_WhenActorDoesNotExist_ThrowsNotFoundException()
    {
        var dto = new ActorRequestDto { FirstName = "Missing", LastName = "Actor" };
        _actorRepository.Setup(r => r.GetActorByIdAsync(100)).ReturnsAsync((Actor?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateActorAsync(100, dto));
    }
}
