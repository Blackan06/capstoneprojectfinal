using DataAccess.Repositories.TaskRepositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTHCMAdventures.Application.UnitTests.Mocks.TaskRepository
{
    public class MockTaskRepository
    {
        public static Mock<ITaskRepositories> GetMockTaskRepository()
        {
            var tasks = new List<BusinessObjects.Model.Task>
            {
                new BusinessObjects.Model.Task
                {
                    Id = Guid.NewGuid(), 
                    LocationId = Guid.NewGuid(), 
                    MajorId = null,
                    NpcId = Guid.NewGuid(), 
                    ItemId = null, 
                    Name = "TaskCheckIn", 
                    Type = "CHECKIN", 
                    Status = "ACTIVE", 
                    CreatedAt = DateTime.UtcNow 
                },
                new BusinessObjects.Model.Task
                {
                    Id = Guid.NewGuid(),
                    LocationId = Guid.NewGuid(),
                    MajorId = null,
                    NpcId = Guid.NewGuid(),
                    ItemId = null,
                    Name = "TaskWithItem",
                    Type = "EXCHANGEITEM",
                    Status = "ACTIVE",
                    CreatedAt = DateTime.UtcNow
                },
                new BusinessObjects.Model.Task
                {
                    Id = Guid.NewGuid(),
                    LocationId = Guid.NewGuid(),
                    MajorId = null,
                    NpcId = Guid.NewGuid(),
                    ItemId = null,
                    Name = "TaskWithQuestionAndAnswer",
                    Type = "QUESTIONANDANSWER",
                    Status = "ACTIVE",
                    CreatedAt = DateTime.UtcNow
                },
                new BusinessObjects.Model.Task
                {
                    Id = Guid.NewGuid(),
                    LocationId = Guid.NewGuid(),
                    MajorId = Guid.NewGuid(),
                    NpcId = Guid.NewGuid(),
                    ItemId = null,
                    Name = "TaskWithMiniGame",
                    Type = "MINIGAME",
                    Status = "ACTIVE",
                    CreatedAt = DateTime.UtcNow
                },

        };

            var mockRepo = new Mock<ITaskRepositories>();

            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            mockRepo.Setup(r => r.CreateAsync(It.IsAny<BusinessObjects.Model.Task>()))
             .Returns((BusinessObjects.Model.Task task) =>
             {
                 tasks.Add(task);
                 return Task.CompletedTask;
             });
            mockRepo.Setup(r => r.IsTaskUnique(It.IsAny<string>()))
                .ReturnsAsync((string name) => {
                    return !tasks.Any(q => q.Name == name);
                });

            return mockRepo;
        }

    }
}
