using AutoMapper;
using Moq;
using Xunit;
using NZWalks.API.Controllers;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace NZWalks.Tests
{
    public class WalksControllerTests
    {
        private readonly Mock<IWalkRepository> _mockWalkRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly WalksController _walksController;

        public WalksControllerTests()
        {
            _mockWalkRepo = new Mock<IWalkRepository>();
            _mockMapper = new Mock<IMapper>();
            _walksController = new WalksController(_mockMapper.Object, _mockWalkRepo.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithListOfWalks()
        {
            // Arrange
            var walks = new List<Walk>
        {
            new Walk { Id = Guid.NewGuid(), Name = "Walk1", LengthInKm = 5.0 },
            new Walk { Id = Guid.NewGuid(), Name = "Walk2", LengthInKm = 10.0 }
        };
            var walkDtos = new List<WalkDto>
        {
            new WalkDto { Id = Guid.NewGuid(), Name = "Walk1", LengthInKm = 5.0 },
            new WalkDto { Id = Guid.NewGuid(), Name = "Walk2", LengthInKm = 10.0 }
        };

            _mockWalkRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(walks);
            _mockMapper.Setup(m => m.Map<List<WalkDto>>(walks)).Returns(walkDtos);

            // Act
            var result = await _walksController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<WalkDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkResult_WithWalkDto()
        {
            // Arrange
            var walkId = Guid.NewGuid();
            var walk = new Walk { Id = walkId, Name = "Walk1", LengthInKm = 8.5 };
            var walkDto = new WalkDto { Id = walkId, Name = "Walk2", LengthInKm = 8.5 };

            _mockWalkRepo.Setup(repo => repo.GetByIdAsync(walkId)).ReturnsAsync(walk);
            _mockMapper.Setup(m => m.Map<WalkDto>(walk)).Returns(walkDto);

            // Act
            var result = await _walksController.GetById(walkId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<WalkDto>(okResult.Value);
            Assert.Equal(walkId, returnValue.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenWalkDoesNotExist()
        {
            // Arrange
            var walkId = Guid.NewGuid();
            _mockWalkRepo.Setup(repo => repo.GetByIdAsync(walkId)).ReturnsAsync((Walk)null);

            // Act
            var result = await _walksController.GetById(walkId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnOkResult_WhenWalkIsAdded()
        {
            // Arrange
            var addWalkRequestDto = new AddWalkRequestDto { Name = "New Walk", LengthInKm = 5.5 };
            var walk = new Walk { Id = Guid.NewGuid(), Name = "New Walk", LengthInKm = 5.5 };
            var walkDto = new WalkDto { Id = walk.Id, Name = "New Walk", LengthInKm = 5.5 };

            _mockMapper.Setup(m => m.Map<Walk>(addWalkRequestDto)).Returns(walk);
            _mockWalkRepo.Setup(repo => repo.CreateAsync(walk)).ReturnsAsync(walk);
            _mockMapper.Setup(m => m.Map<WalkDto>(walk)).Returns(walkDto);

            // Act
            var result = await _walksController.Create(addWalkRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<WalkDto>(okResult.Value);
            Assert.Equal(walk.Id, returnValue.Id);
        }


        [Fact]
        public async Task Update_ShouldReturnOkResult_WhenWalkIsUpdated()
        {
            // Arrange
            var walkId = Guid.NewGuid();
            var updateWalkRequestDto = new UpdateWalkRequestDto { Name = "Updated Walk", LengthInKm = 10.0 };
            var walk = new Walk { Id = walkId, Name = "Updated Walk", LengthInKm = 10.0 };
            var walkDto = new WalkDto { Id = walkId, Name = "Updated Walk", LengthInKm = 10.0 };

            _mockMapper.Setup(m => m.Map<Walk>(updateWalkRequestDto)).Returns(walk);
            _mockWalkRepo.Setup(repo => repo.UpdateAsync(walkId, walk)).ReturnsAsync(walk);
            _mockMapper.Setup(m => m.Map<WalkDto>(walk)).Returns(walkDto);

            // Act
            var result = await _walksController.Update(walkId, updateWalkRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<WalkDto>(okResult.Value);
            Assert.Equal(walkId, returnValue.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenWalkDoesNotExist()
        {
            // Arrange
            var walkId = Guid.NewGuid();
            var updateWalkRequestDto = new UpdateWalkRequestDto { Name = "Updated Walk", LengthInKm = 10.0 };
            _mockWalkRepo.Setup(repo => repo.UpdateAsync(walkId, It.IsAny<Walk>())).ReturnsAsync((Walk)null);

            // Act
            var result = await _walksController.Update(walkId, updateWalkRequestDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnOkResult_WhenWalkIsDeleted()
        {
            // Arrange
            var walkId = Guid.NewGuid();
            var walk = new Walk { Id = walkId, Name = "deleted", LengthInKm = 7.5 };
            var walkDto = new WalkDto { Id = walkId, Name = "deleted", LengthInKm = 7.5 };

            _mockWalkRepo.Setup(repo => repo.DeleteAsync(walkId)).ReturnsAsync(walk);
            _mockMapper.Setup(m => m.Map<WalkDto>(walk)).Returns(walkDto);

            // Act
            var result = await _walksController.Delete(walkId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<WalkDto>(okResult.Value);
            Assert.Equal(walkId, returnValue.Id);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenWalkDoesNotExist()
        {
            // Arrange
            var walkId = Guid.NewGuid();
            _mockWalkRepo.Setup(repo => repo.DeleteAsync(walkId)).ReturnsAsync((Walk)null);

            // Act
            var result = await _walksController.Delete(walkId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithEmptyList()
        {
            // Arrange
            var emptyList = new List<Walk>();
            _mockWalkRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<List<WalkDto>>(emptyList)).Returns(new List<WalkDto>());

            // Act
            var result = await _walksController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<WalkDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task Create_ShouldMapAddWalkRequestDtoToWalk()
        {
            // Arrange
            var addWalkRequestDto = new AddWalkRequestDto { Name = "Test Walk", LengthInKm = 5.0 };
            var walk = new Walk { Id = Guid.NewGuid(), Name = "Test Walk", LengthInKm = 5.0 };

            _mockMapper.Setup(m => m.Map<Walk>(addWalkRequestDto)).Returns(walk);

            // Act
            var result = await _walksController.Create(addWalkRequestDto);

            // Assert
            _mockMapper.Verify(m => m.Map<Walk>(addWalkRequestDto), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldMapUpdateWalkRequestDtoToWalk()
        {
            // Arrange
            var updateWalkRequestDto = new UpdateWalkRequestDto { Name = "Updated Walk", LengthInKm = 10.0 };
            var walk = new Walk { Id = Guid.NewGuid(), Name = "Updated Walk", LengthInKm = 10.0 };

            _mockMapper.Setup(m => m.Map<Walk>(updateWalkRequestDto)).Returns(walk);

            // Act
            var result = await _walksController.Update(walk.Id, updateWalkRequestDto);

            // Assert
            _mockMapper.Verify(m => m.Map<Walk>(updateWalkRequestDto), Times.Once);
        }
    }
}
