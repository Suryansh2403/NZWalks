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
    public class RegionsControllerTests
    {
        private readonly Mock<IRegionRepository> _mockRegionRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly RegionsController _regionsController;

        public RegionsControllerTests()
        {
            _mockRegionRepo = new Mock<IRegionRepository>();
            _mockMapper = new Mock<IMapper>();
            _regionsController = new RegionsController(_mockRegionRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllRegions_ShouldReturnOkResult_WithListOfRegions()
        {
            // Arrange
            var regions = new List<Region>
        {
            new Region { Id = Guid.NewGuid(), Name = "Region1", Code = "RG1", RegionImageUrl = "url1"},
            new Region { Id = Guid.NewGuid(), Name = "Region2", Code = "RG2", RegionImageUrl = "url2"}
        };
            var regionDtos = new List<RegionDto>
        {
            new RegionDto { Id = Guid.NewGuid(), Name = "Region1", Code = "RG1", RegionImageUrl = "url1" },
            new RegionDto { Id = Guid.NewGuid(), Name = "Region2", Code = "RG2", RegionImageUrl = "url2" }
        };

            _mockRegionRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(regions);
            _mockMapper.Setup(m => m.Map<List<RegionDto>>(regions)).Returns(regionDtos);

            // Act
            var result = await _regionsController.GetAllRegions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<RegionDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }


        [Fact]
        public async Task GetAllRegions_ShouldReturnOkResult_WithEmptyList()
        {
            // Arrange
            var emptyList = new List<Region>();
            _mockRegionRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<List<RegionDto>>(emptyList)).Returns(new List<RegionDto>());

            // Act
            var result = await _regionsController.GetAllRegions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<RegionDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetRegionById_ShouldReturnOkResult_WithRegionDto()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var region = new Region { Id = regionId, Name = "TestRegion", Code = "TR" };
            var regionDto = new RegionDto { Id = regionId, Name = "TestRegion", Code = "TR", RegionImageUrl = "url" };

            _mockRegionRepo.Setup(repo => repo.GetByIdAsync(regionId)).ReturnsAsync(region);
            _mockMapper.Setup(m => m.Map<RegionDto>(region)).Returns(regionDto);

            // Act
            var result = await _regionsController.GetRegionById(regionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RegionDto>(okResult.Value);
            Assert.Equal(regionId, returnValue.Id);
        }

        [Fact]
        public async Task GetRegionById_ShouldReturnNotFound_WhenRegionDoesNotExist()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            _mockRegionRepo.Setup(repo => repo.GetByIdAsync(regionId)).ReturnsAsync((Region)null);

            // Act
            var result = await _regionsController.GetRegionById(regionId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateRegion_ShouldReturnCreatedAtAction_WhenRegionIsCreated()
        {
            // Arrange
            var addRegionRequestDto = new AddRegionRequestDto { Code = "TRT", Name = "TestRegion", RegionImageUrl = "url" };
            var region = new Region { Id = Guid.NewGuid(), Code = "TRT", Name = "TestRegion", RegionImageUrl = "url" };
            var regionDto = new RegionDto { Id = region.Id, Code = "TRT", Name = "TestRegion", RegionImageUrl = "url" };

            _mockMapper.Setup(m => m.Map<Region>(addRegionRequestDto)).Returns(region);
            _mockRegionRepo.Setup(repo => repo.CreateAsync(region)).ReturnsAsync(region);
            _mockMapper.Setup(m => m.Map<RegionDto>(region)).Returns(regionDto);

            // Act
            var result = await _regionsController.Create(addRegionRequestDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<RegionDto>(createdAtActionResult.Value);
            Assert.Equal(region.Id, returnValue.Id);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var addRegionRequestDto = new AddRegionRequestDto { Code = "", Name = "", RegionImageUrl = "" }; // Invalid data
            _regionsController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _regionsController.Create(addRegionRequestDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnOkResult_WhenRegionIsUpdated()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var updateRegionRequestDto = new UpdateRegionRequestDto { Code = "URG", Name = "UpdatedRegion", RegionImageUrl = "url" };
            var region = new Region { Id = regionId, Code = "URG", Name = "UpdatedRegion", RegionImageUrl = "url" };
            var regionDto = new RegionDto { Id = regionId, Code = "URG", Name = "UpdatedRegion", RegionImageUrl = "url" };

            _mockMapper.Setup(m => m.Map<Region>(updateRegionRequestDto)).Returns(region);
            _mockRegionRepo.Setup(repo => repo.UpdateAsync(regionId, region)).ReturnsAsync(region);
            _mockMapper.Setup(m => m.Map<RegionDto>(region)).Returns(regionDto);

            // Act
            var result = await _regionsController.Update(regionId, updateRegionRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RegionDto>(okResult.Value);
            Assert.Equal(regionId, returnValue.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenRegionDoesNotExist()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var updateRegionRequestDto = new UpdateRegionRequestDto { Code = "URG", Name = "UpdatedRegion", RegionImageUrl = "url" };
            _mockRegionRepo.Setup(repo => repo.UpdateAsync(regionId, It.IsAny<Region>())).ReturnsAsync((Region)null);

            // Act
            var result = await _regionsController.Update(regionId, updateRegionRequestDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnOkResult_WhenRegionIsDeleted()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var region = new Region { Id = regionId, Code = "TRT", Name = "TestRegion", RegionImageUrl = "url" };
            var regionDto = new RegionDto { Id = regionId, Code = "TRT", Name = "TestRegion", RegionImageUrl = "url" };

            _mockRegionRepo.Setup(repo => repo.DeleteAsync(regionId)).ReturnsAsync(region);
            _mockMapper.Setup(m => m.Map<RegionDto>(region)).Returns(regionDto);

            // Act
            var result = await _regionsController.Delete(regionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RegionDto>(okResult.Value);
            Assert.Equal(regionId, returnValue.Id);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenRegionDoesNotExist()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            _mockRegionRepo.Setup(repo => repo.DeleteAsync(regionId)).ReturnsAsync((Region)null);

            // Act
            var result = await _regionsController.Delete(regionId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
