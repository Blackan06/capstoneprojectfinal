using AutoMapper;
using BusinessObjects.Model;
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
    
    public class UpdateTaskServiceTest
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

        public UpdateTaskServiceTest()
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
        public async System.Threading.Tasks.Task UpdateTask_WithValidInput_ShouldReturnSuccessResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto
            {
                LocationName = "Location1",
                MajorName = "Major1",
                NpcName = "Npc1",
                ItemName = "Item1",
                Name = "UpdatedTask",
                Type = "CHECKIN",
                Status = "ACTIVE"
            };

            var existingTask = new BusinessObjects.Model.Task
            {
                Id = taskId,
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = Guid.NewGuid(),
                Name = "TaskToBeUpdated",
                Type = "CHECKIN",
                Status = "INACTIVE"
            };

            _taskRepoMock.Setup(r => r.GetById(taskId)).ReturnsAsync(existingTask);
            _taskRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<BusinessObjects.Model.Task, bool>>>()))
                            .ReturnsAsync(false);
            _locationRepoMock.Setup(r => r.GetLocationByName(It.IsAny<string>())).ReturnsAsync(new Location());
            _majorRepoMock.Setup(r => r.GetMajorByName(It.IsAny<string>())).ReturnsAsync(new Major());
            _npcRepoMock.Setup(r => r.GetNpcDTOByName(It.IsAny<string>())).ReturnsAsync(new Npc());
            _itemRepoMock.Setup(r => r.GetItemByName(It.IsAny<string>())).ReturnsAsync(new Item());



            // Act
            var result = await _sut.UpdateTask(taskId, updateTaskDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(202, result.StatusCode);
            // Add additional assertions to verify that the task was updated as expected
        }  
        [Fact]
        public async System.Threading.Tasks.Task UpdateTask_WithValidInputStatus_ShouldReturnSuccessResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto
            {
                LocationName = "Location1",
                MajorName = "Major1",
                NpcName = "Npc1",
                ItemName = "Item1",
                Name = "UpdatedTask",
                Type = "CHECKIN",
                Status = "ACTIVE"
            };

            var existingTask = new BusinessObjects.Model.Task
            {
                Id = taskId,
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = Guid.NewGuid(),
                Name = "TaskToBeUpdated",
                Type = "CHECKIN",
                Status = "ACTIVE"
            };

            _taskRepoMock.Setup(r => r.GetById(taskId)).ReturnsAsync(existingTask);
            _taskRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<BusinessObjects.Model.Task, bool>>>()))
                            .ReturnsAsync(false);
            _locationRepoMock.Setup(r => r.GetLocationByName(It.IsAny<string>())).ReturnsAsync(new Location());
            _majorRepoMock.Setup(r => r.GetMajorByName(It.IsAny<string>())).ReturnsAsync(new Major());
            _npcRepoMock.Setup(r => r.GetNpcDTOByName(It.IsAny<string>())).ReturnsAsync(new Npc());
            _itemRepoMock.Setup(r => r.GetItemByName(It.IsAny<string>())).ReturnsAsync(new Item());



            // Act
            var result = await _sut.UpdateTask(taskId, updateTaskDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(202, result.StatusCode);
            // Add additional assertions to verify that the task was updated as expected
        }
        [Fact]
        public async System.Threading.Tasks.Task UpdateTask_WithValidInput_CheckinType_ShouldReturnSuccessResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto
            {
                LocationName = "Location1",
                MajorName = "Major1",
                NpcName = "Npc1",
                ItemName = "Item1",
                Name = "UpdatedTask",
                Type = "CHECKIN",
                Status = "ACTIVE"
            };

            var existingTask = new BusinessObjects.Model.Task
            {
                Id = taskId,
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = Guid.NewGuid(),
                Name = "TaskToBeUpdated",
                Type = "CHECKIN",
                Status = "ACTIVE"
            };

            _taskRepoMock.Setup(r => r.GetById(taskId)).ReturnsAsync(existingTask);
            _taskRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<BusinessObjects.Model.Task, bool>>>()))
                .ReturnsAsync(false);
            _locationRepoMock.Setup(r => r.GetLocationByName(It.IsAny<string>())).ReturnsAsync(new Location());
            _majorRepoMock.Setup(r => r.GetMajorByName(It.IsAny<string>())).ReturnsAsync(new Major());
            _npcRepoMock.Setup(r => r.GetNpcDTOByName(It.IsAny<string>())).ReturnsAsync(new Npc());
            _itemRepoMock.Setup(r => r.GetItemByName(It.IsAny<string>())).ReturnsAsync(new Item());

            // Act
            var result = await _sut.UpdateTask(taskId, updateTaskDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(202, result.StatusCode);
            // Add additional assertions to verify that the task was updated as expected
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTask_WithValidInput_MinigameType_ShouldReturnSuccessResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto
            {
                LocationName = "Location1",
                MajorName = "Major1",
                NpcName = "Npc1",
                ItemName = "Item1",
                Name = "UpdatedTask",
                Type = "MINIGAME",
                Status = "ACTIVE"
            };

            var existingTask = new BusinessObjects.Model.Task
            {
                Id = taskId,
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = Guid.NewGuid(),
                Name = "TaskToBeUpdated",
                Type = "MINIGAME",
                Status = "ACTIVE"
            };

            _taskRepoMock.Setup(r => r.GetById(taskId)).ReturnsAsync(existingTask);
            _taskRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<BusinessObjects.Model.Task, bool>>>()))
                .ReturnsAsync(false);
            _locationRepoMock.Setup(r => r.GetLocationByName(It.IsAny<string>())).ReturnsAsync(new Location());
            _majorRepoMock.Setup(r => r.GetMajorByName(It.IsAny<string>())).ReturnsAsync(new Major());
            _npcRepoMock.Setup(r => r.GetNpcDTOByName(It.IsAny<string>())).ReturnsAsync(new Npc());
            _itemRepoMock.Setup(r => r.GetItemByName(It.IsAny<string>())).ReturnsAsync(new Item());

            // Act
            var result = await _sut.UpdateTask(taskId, updateTaskDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(202, result.StatusCode);
            // Add additional assertions to verify that the task was updated as expected
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTask_WithValidInput_QuestionAndAnswerType_ShouldReturnSuccessResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto
            {
                LocationName = "Location1",
                MajorName = "Major1",
                NpcName = "Npc1",
                ItemName = "Item1",
                Name = "UpdatedTask",
                Type = "QUESTIONANDANSWER",
                Status = "ACTIVE"
            };

            var existingTask = new BusinessObjects.Model.Task
            {
                Id = taskId,
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = Guid.NewGuid(),
                Name = "TaskToBeUpdated",
                Type = "QUESTIONANDANSWER",
                Status = "ACTIVE"
            };

            _taskRepoMock.Setup(r => r.GetById(taskId)).ReturnsAsync(existingTask);
            _taskRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<BusinessObjects.Model.Task, bool>>>()))
                .ReturnsAsync(false);
            _locationRepoMock.Setup(r => r.GetLocationByName(It.IsAny<string>())).ReturnsAsync(new Location());
            _majorRepoMock.Setup(r => r.GetMajorByName(It.IsAny<string>())).ReturnsAsync(new Major());
            _npcRepoMock.Setup(r => r.GetNpcDTOByName(It.IsAny<string>())).ReturnsAsync(new Npc());
            _itemRepoMock.Setup(r => r.GetItemByName(It.IsAny<string>())).ReturnsAsync(new Item());
            _questionRepoMock.Setup(r => r.GetByMajorIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Question());

            // Act
            var result = await _sut.UpdateTask(taskId, updateTaskDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(202, result.StatusCode);
            // Add additional assertions to verify that the task was updated as expected
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTask_WithValidInput_ExchangeItemType_ShouldReturnSuccessResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateTaskDto = new UpdateTaskDto
            {
                LocationName = "Location1",
                MajorName = "Major1",
                NpcName = "Npc1",
                ItemName = "Item1",
                Name = "UpdatedTask",
                Type = "EXCHANGEITEM",
                Status = "ACTIVE"
            };

            var existingTask = new BusinessObjects.Model.Task
            {
                Id = taskId,
                LocationId = Guid.NewGuid(),
                MajorId = Guid.NewGuid(),
                NpcId = Guid.NewGuid(),
                ItemId = Guid.NewGuid(),
                Name = "TaskToBeUpdated",
                Type = "EXCHANGEITEM",
                Status = "ACTIVE"
            };

            _taskRepoMock.Setup(r => r.GetById(taskId)).ReturnsAsync(existingTask);
            _taskRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<BusinessObjects.Model.Task, bool>>>()))
                .ReturnsAsync(false);
            _locationRepoMock.Setup(r => r.GetLocationByName(It.IsAny<string>())).ReturnsAsync(new Location());
            _majorRepoMock.Setup(r => r.GetMajorByName(It.IsAny<string>())).ReturnsAsync(new Major());
            _npcRepoMock.Setup(r => r.GetNpcDTOByName(It.IsAny<string>())).ReturnsAsync(new Npc());
            _itemRepoMock.Setup(r => r.GetItemByName(It.IsAny<string>())).ReturnsAsync(new Item());

            // Act
            var result = await _sut.UpdateTask(taskId, updateTaskDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(202, result.StatusCode);
            // Add additional assertions to verify that the task was updated as expected
        }
    }
}
    