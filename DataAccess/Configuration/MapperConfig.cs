using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.InventoryDto;
using DataAccess.Dtos.ItemDto;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.Dtos.LocationDto;
using DataAccess.Dtos.MajorDto;
using DataAccess.Dtos.NPCDto;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.Dtos.PlayerPrizeDto;
using DataAccess.Dtos.PrizeDto;
using DataAccess.Dtos.QuestionDto;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.Dtos.StudentDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DataAccess.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            #region Event
            CreateMap<Event, GetEventDto>().ReverseMap();
            CreateMap<Event, UpdateEventDto>().ReverseMap();
            CreateMap<Event, CreateEventDto>().ReverseMap();
            CreateMap<Event, EventDto>().ReverseMap();
            #endregion


            #region Task
            CreateMap<Task, GetTaskDto>().ReverseMap();

            CreateMap<Task, UpdateTaskDto>().ReverseMap();

            CreateMap<Task, CreateTaskDto>().ReverseMap();
                      

            CreateMap<Task, TaskDto>()
                       .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name))
                       .ForMember(dest => dest.NpcName, opt => opt.MapFrom(src => src.Npc.Name))
                       .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                       .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.Name));
            CreateMap<TaskDto, Task>()
                        .ForMember(dest => dest.Major, opt => opt.Ignore())
                        .ForMember(dest => dest.Npc, opt => opt.Ignore())
                        .ForMember(dest => dest.Location, opt => opt.Ignore())
                        .ForMember(dest => dest.Item, opt => opt.Ignore());
            #endregion

            #region Player Prize
            CreateMap<PlayerPrize, GetPrizeDto>().ReverseMap();
            CreateMap<PlayerPrize, UpdatePlayerPrizeDto>().ReverseMap();
            CreateMap<PlayerPrize, CreatePlayerPrizeDto>().ReverseMap();
            CreateMap<PlayerPrize, PlayerPrizeDto>()
                        .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player.Nickname))
                        .ForMember(dest => dest.PrizeName, opt => opt.MapFrom(src => src.Prize.Name));
            #endregion
            #region Event Task
            CreateMap<EventTask, GetEventTaskDto>().ReverseMap();

            CreateMap<EventTask, UpdateEventTaskDto>().ReverseMap();

            CreateMap<EventTask, CreateEventTaskDto>().ReverseMap();

            CreateMap<EventTask, EventTaskDto>()
                       .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                       .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Task.Name));
            CreateMap<CreateListEventTaskDto, EventTask>()
                       .ForMember(dest => dest.Id, opt => opt.Ignore())
                       .ForMember(dest => dest.PlayerHistories, opt => opt.Ignore())
                       .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Không ánh xạ CreatedAt từ DTO vì nó sẽ được tạo trong logic xử lý Entity

                       .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                       .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                       .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                       .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                       .ForMember(dest => dest.Point, opt => opt.MapFrom(src => src.Point))
                       .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId.FirstOrDefault()));// Ánh xạ TaskId từ List<Guid> trong DTO sang Guid trong Entity
                      
            #endregion

            #region Major
            CreateMap<Major, GetMajorDto>().ReverseMap();
            CreateMap<Major, UpdateMajorDto>().ReverseMap();
            CreateMap<Major, CreateMajorDto>().ReverseMap();
            CreateMap<Major, MajorDto>().ReverseMap();
            #endregion  
            
            
            #region Question
            CreateMap<Question, GetQuestionDto>().ReverseMap();
            CreateMap<Question, UpdateQuestionDto>().ReverseMap();
            CreateMap<Question, CreateQuestionDto>().ReverseMap();
            CreateMap<Question, QuestionDto>()
                            .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name));

            #endregion  
            
            #region Student
            CreateMap<Student, GetStudentDto>().ReverseMap();
            CreateMap<Student, UpdateStudentDto>().ReverseMap();
            CreateMap<Student, CreateStudentDto>().ReverseMap();
            CreateMap<Student, StudentDto>()
                            .ForMember(dest => dest.Schoolname, opt => opt.MapFrom(src => src.School.Name))
                            .ForMember(dest => dest.PlayerNickName, opt => opt.MapFrom(src => src.Player.Nickname));

            #endregion

            #region NPC
            CreateMap<Npc, GetNpcDto>().ReverseMap();
            CreateMap<Npc, UpdateNpcDto>().ReverseMap();
            CreateMap<Npc, CreateNpcDto>().ReverseMap();
            CreateMap<Npc, NpcDto>().ReverseMap();
            #endregion 
            
            #region Location
            CreateMap<Location, GetLocationDto>().ReverseMap();
            CreateMap<Location, UpdateLocationDto>().ReverseMap();
            CreateMap<Location, CreateLocationDto>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
            #endregion


            #region Answer
            CreateMap<Answer, AnswerDto>()
                            .ForMember(dest => dest.QuestionName, opt => opt.MapFrom(src => src.Question.Name));
            CreateMap<Answer, UpdateAnswerDto>().ReverseMap();
            CreateMap<Answer, GetAnswerListDto>().ReverseMap();
            CreateMap<Answer, GetAnswerDto>().ReverseMap();
            CreateMap<Answer, CreateAnswerDto>().ReverseMap();

            #endregion

            #region Prize
            CreateMap<Prize, PrizeDto>()
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name));
            CreateMap<Prize, UpdatePrizeDto>().ReverseMap();
            CreateMap<Prize, CreatePrizeDto>().ReverseMap();
            CreateMap<Prize, GetPrizeDto>().ReverseMap();
            #endregion

            #region Exchange History
            CreateMap<ExchangeHistory, ExchangeHistoryDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player.Nickname))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.Name));
            CreateMap<ExchangeHistory, UpdateExchangeHistoryDto>().ReverseMap();
            CreateMap<ExchangeHistory, CreateExchangeHistoryDto>().ReverseMap();
            CreateMap<ExchangeHistory, GetExchangeHistoryDto>().ReverseMap();
            #endregion

            #region Inventory
            CreateMap<Inventory, InventoryDto>()
                  .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player.Nickname));
            CreateMap<Inventory, UpdateInventoryDto>().ReverseMap();
            CreateMap<Inventory, CreateInventoryDto>().ReverseMap();
            CreateMap<Inventory, GetInventoryDto>().ReverseMap();
            #endregion

            #region Item Inventory
            CreateMap<ItemInventory, ItemInventoryDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.Name));
                

            CreateMap<ItemInventory, UpdateItemInventoryDto>().ReverseMap();
            CreateMap<ItemInventory, CreateItemInventoryDto>().ReverseMap();
            CreateMap<ItemInventory, GetItemInventoryDto>().ReverseMap();
            #endregion

            #region Player
            CreateMap<Player, PlayerDto>()
                            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.Fullname))
                            .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.Email))
                            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.Student.School.Name));
            CreateMap<Player, UpdatePlayerDto>().ReverseMap();
            CreateMap<Player, CreatePlayerDto>().ReverseMap();
            CreateMap<Player, GetPlayerDto>().ReverseMap();
            CreateMap<Player, GetPlayerWithSchoolAndEvent>().ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Student.Email))
                                                            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.Student.School.Name));
            CreateMap<CreateListPlayerDto, Player>()
                           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                           .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                           .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
                           .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.Nickname))
                           .ForMember(dest => dest.Passcode, opt => opt.MapFrom(src => src.Passcode))
                           .ForMember(dest => dest.TotalPoint, opt => opt.MapFrom(src => src.TotalPoint))
                           .ForMember(dest => dest.TotalTime, opt => opt.MapFrom(src => src.TotalTime))
                           .ForMember(dest => dest.Isplayer, opt => opt.MapFrom(src => src.IsPlayer))
                           .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            #endregion

            #region Player History
            CreateMap<PlayerHistory, PlayerHistoryDto>()
                            .ForMember(dest => dest.PlayerNickName, opt => opt.MapFrom(src => src.Player.Nickname));
            CreateMap<PlayerHistory, UpdatePlayerHistoryDto>().ReverseMap();
            CreateMap<PlayerHistory, CreatePlayerHistoryDto>().ReverseMap();
            CreateMap<PlayerHistory, GetPlayerHistoryDto>().ReverseMap();
            #endregion
            
            
            
            #region School
            CreateMap<School, SchoolDto>().ReverseMap();
            CreateMap<School, GetSchoolDto>().ReverseMap();
            CreateMap<School, CreateSchoolDto>().ReverseMap();
            CreateMap<School, UpdateSchoolDto>().ReverseMap();
            #endregion
            
            #region School Event
            CreateMap<SchoolEvent, SchoolEventDto>()
                            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School.Name))
                            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name));
            CreateMap<SchoolEvent, GetSchoolEventDto>().ReverseMap();
            CreateMap<SchoolEvent, CreateSchoolEventDto>().ReverseMap();
            CreateMap<SchoolEvent, TimeSchoolEventDTO>().ReverseMap();
            CreateMap<SchoolEvent, UpdateSchoolEventDto>().ReverseMap();
            CreateMap<CreateListSchoolEvent, SchoolEvent>()
                           .ForMember(dest => dest.Id, opt => opt.Ignore())
                           .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Không ánh xạ CreatedAt từ DTO vì nó sẽ được tạo trong logic xử lý Entity

                           .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                           .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                           .ForMember(dest => dest.ApprovalStatus, opt => opt.MapFrom(src => src.ApprovalStatus))
                           .ForMember(dest => dest.SchoolId, opt => opt.MapFrom(src => src.SchoolIds.FirstOrDefault()));
            #endregion



            #region Item
            CreateMap<Item, ItemDto>().ReverseMap();
            CreateMap<Item, GetListItemDto>().ReverseMap();
            CreateMap<Item, GetItemDto>().ReverseMap();
            CreateMap<Item, CreateItemDto>().ReverseMap();
            CreateMap<Item, UpdateItemDto>().ReverseMap();
            #endregion

            #region StudentLogin
            CreateMap<Student, ApiUserDto>().ReverseMap();
            CreateMap<Student, AuthResponseDto>().ReverseMap();
            CreateMap<Student, LoginDto>().ReverseMap();
            #endregion






        }
    }
}
