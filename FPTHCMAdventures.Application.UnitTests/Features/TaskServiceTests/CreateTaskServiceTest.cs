using AutoMapper;
using DataAccess.Configuration;
using DataAccess.Dtos.TaskDto;
using DataAccess.Repositories.EventRepositories;
using DataAccess.Repositories.EventTaskRepositories;
using DataAccess.Repositories.ItemRepositories;
using DataAccess.Repositories.LocationRepositories;
using DataAccess.Repositories.MajorRepositories;
using DataAccess.Repositories.NPCRepository;
using DataAccess.Repositories.QuestionRepositories;
using DataAccess.Repositories.TaskRepositories;
using FPTHCMAdventures.Application.UnitTests.Mocks.TaskRepository;
using Moq;
using Service.Services.TaskService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FPTHCMAdventures.Application.UnitTests.Features.TaskServiceTests
{
    public class CreateTaskServiceTest
    {
        private readonly TaskService _sut;
        private readonly Mock<ITaskRepositories> _taskRepoMock = new Mock<ITaskRepositories>();
        private readonly Mock<IEventRepositories> _eventRepoMock = new Mock<IEventRepositories>();
        private readonly Mock<IEventTaskRepository> _eventTaskRepoMock = new Mock<IEventTaskRepository>();
        private readonly Mock<IMajorRepository> _majorRepoMock = new Mock<IMajorRepository>();
        private readonly Mock<INpcRepository> _npcRepoMock = new Mock<INpcRepository>();
        private readonly Mock<IItemRepository> _itemRepoMock = new Mock<IItemRepository>();
        private readonly Mock<ILocationRepository> _locationRepoMock = new Mock<ILocationRepository>();
        private readonly Mock<IQuestionRepository> _questionRepoMock = new Mock<IQuestionRepository>();
        private readonly IMapper _mapper;

        public CreateTaskServiceTest()
        {
            _taskRepoMock = MockTaskRepository.GetMockTaskRepository();

            _sut = new TaskService(
                           _itemRepoMock.Object,
                           _taskRepoMock.Object,
                           _mapper,
                           _eventTaskRepoMock.Object,
                           _eventRepoMock.Object,
                           _majorRepoMock.Object,
                           _questionRepoMock.Object,
                           _locationRepoMock.Object,
                           _npcRepoMock.Object
                       );
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<MapperConfig>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task CreateNewTask_WithValidInput_ShouldReturnSuccessResponse()
        {
            var createTaskDto = new CreateTaskDto
            {
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = null,
                Name = "ValidTask",
                Type = "CHECKIN",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _sut.CreateNewTask(createTaskDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);

            // Verify that the task was added to the list
        }
        [Fact]
        public async Task CreateNewTask_WithDuplicateName_ShouldReturnErrorResponse()
        {
            // Arrange
            var existingTask = new BusinessObjects.Model.Task
            {
                Id = Guid.NewGuid(),
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = null,
                Name = "TaskCheckIn",
                Type = "CHECKIN",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            _taskRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<BusinessObjects.Model.Task, bool>>>()))
                .ReturnsAsync(true);


            var createTaskDto = new CreateTaskDto
            {
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                Name = "TaskCheckIn", // Duplicate name
                Type = "CHECKIN",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _sut.CreateNewTask(createTaskDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Duplicated data: Task with the same name already exists.", result.Message);
        }

        [Fact]
        public async Task CreateNewTask_WithInvalidStatus_ShouldReturnErrorResponse()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                Name = "InvalidStatusTask",
                Type = "CHECKIN",
                Status = "INVALID_STATUS", // Invalid status value
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _sut.CreateNewTask(createTaskDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Status must be 'INACTIVE' or 'ACTIVE'.", result.Message);
        }
        [Fact]
        public async Task CreateNewTask_WithMissingMajor_ShouldReturnErrorResponse()
        {
            // Arrange
            _majorRepoMock.Setup(r => r.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BusinessObjects.Model.Major)null); // Simulate missing major

            var createTaskDto = new CreateTaskDto
            {
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                Name = "ValidTask",
                Type = "QUESTIONANDANSWER", // Set type to "QUESTIONANDANSWER"
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _sut.CreateNewTask(createTaskDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Can't add jobs. The corresponding branch was not found.", result.Message);
        }

        [Fact]
        public async Task CreateNewTask_WithMissingQuestions_ShouldReturnErrorResponse()
        {
            // Arrange
            _majorRepoMock.Setup(r => r.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new BusinessObjects.Model.Major()); // Simulate valid major

            _questionRepoMock.Setup(r => r.GetByMajorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BusinessObjects.Model.Question)null); 

            var createTaskDto = new CreateTaskDto
            {
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                Name = "ValidTask",
                Type = "QUESTIONANDANSWER", // Set type to "QUESTIONANDANSWER"
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _sut.CreateNewTask(createTaskDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Can't add jobs. Industry must have a question with an answer.", result.Message);
        }

        [Fact]
        public async Task CreateNewTask_WithMissingItem_ShouldReturnErrorResponse()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                Name = "ValidTask",
                Type = "EXCHANGEITEM", // Set type to "EXCHANGEITEM"
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _sut.CreateNewTask(createTaskDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Must have item", result.Message);
        }

    }
}
