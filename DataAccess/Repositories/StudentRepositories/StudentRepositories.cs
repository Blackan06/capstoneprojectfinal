using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.StudentDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DataAccess.Repositories.StudentRepositories
{
    public class StudentRepositories : GenericRepository<Student>, IStudentRepositories
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public StudentRepositories(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<string>> GetExistingEmails()
        {
            return await _dbContext.Students.Select(s => s.Email).ToListAsync();
        }

        public async Task<List<string>> GetExistingPhoneNumbers()
        {
            return await _dbContext.Students.Select(s => s.Phonenumber.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<GetStudentBySchoolAndEvent>> GetStudentBySchoolId(Guid SchoolId)
        {
            var students = await _dbContext.Students.Include(x => x.School).Where(s => s.SchoolId.Equals(SchoolId)).OrderByDescending(x => x.CreatedAt).Select(x=>new GetStudentBySchoolAndEvent
            {
                Id = x.Id,
                Passcode = x.Player != null ? (x.Player.Passcode ?? "N/A") : "N/A",
                Schoolname = x.School.Name,
                Fullname = x.Fullname,
                Email = x.Email,
                Phonenumber = x.Phonenumber,
                GraduateYear = x.GraduateYear,
                Classname = x.Classname,
                Status = x.Status
            }).ToListAsync();
            return students;
        }

        public async Task<IEnumerable<GetStudentBySchoolAndEvent>> GetStudentByEventSchoolId(Guid schoolEventId)
        {
            
                var schoolEvent = await _dbContext.SchoolEvents.Include(x => x.Event).Include(x => x.School).Where(x => x.Id == schoolEventId).FirstOrDefaultAsync();
                if (schoolEvent != null)
                {
                    
                    var students = await _dbContext.Students.Include(x => x.Player)
                                    .Include(x => x.School).ThenInclude(x => x.SchoolEvents)
                                    .Where(s => s.SchoolId == schoolEvent.SchoolId)
                                    .ToListAsync();
                    var studentDtos = students.Select(x => new GetStudentBySchoolAndEvent
                    {
                        Id = x.Id,
                        EventId = schoolEvent.EventId,
                        EventName = schoolEvent.Event.Name,
                        Passcode = x.Player != null ? (x.Player.Passcode ?? "N/A") : "N/A",
                        Schoolname = x.School.Name,
                        Fullname = x.Fullname,
                        Email = x.Email,
                        Phonenumber = x.Phonenumber,
                        GraduateYear = x.GraduateYear,
                        Classname = x.Classname,
                        Status = x.Status
                    }).ToList().OrderByDescending(x => x.CreatedAt);

                    // You may need to add additional logic here to filter students based on the eventId.

                    return studentDtos;
                }
                else
                {
                    return null;
                }
           
           
        }
        public async Task<IEnumerable<GetStudentWithPlayerDto>> GetStudentsBySchoolIdAsync(Guid schoolId)
        {
            return await _dbContext.Students
                .Where(student => student.SchoolId == schoolId)
                .Select(student => new GetStudentWithPlayerDto
                {
                    // Map properties from Student entity to StudentDto
                    Id = student.Id,
                    Schoolname = student.School.Name,
                    Fullname = student.Fullname,
                    Email = student.Email,
                    Phonenumber = student.Phonenumber,
                    GraduateYear = student.GraduateYear,
                    Classname = student.Classname,
                    Passcode = student.Player.Passcode,
                    Status = student.Status
                })
                .ToListAsync();
        }
        public async Task<bool> CheckStudentInSchoolEvent(Guid studentId, Guid schoolId, Guid eventId)
        {
            // Tìm kiếm sự kiện của trường dựa trên schoolId và eventId
            var schoolEvent = await _dbContext.SchoolEvents.FirstOrDefaultAsync(se => se.SchoolId == schoolId && se.EventId == eventId);

            if (schoolEvent == null)
            {
                return false;
            }

            // Kiểm tra xem học sinh đã tham gia sự kiện của trường hay chưa
            var studentParticipation = await _dbContext.Students
                .FirstOrDefaultAsync(sp => sp.Id == studentId && sp.SchoolId == schoolEvent.SchoolId);

            return studentParticipation != null;
        }
        public async Task UpdateStudentStatusAsync(Guid studentId, string newStatus)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);

            if (student != null)
            {
                student.Status = newStatus;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<GetStudentBySchoolAndEvent>> filterData(Guid? schoolId, Guid? eventId)
        {
            IQueryable<Student> query = _dbContext.Students.Include(s => s.School)
                                                            .ThenInclude(school => school.SchoolEvents)
                                                            .ThenInclude(schoolEvent => schoolEvent.Event)
                                                            .Include(x => x.Player); 
            if (schoolId.HasValue)
            {
                query = query.Where(s => s.SchoolId == schoolId.Value);
            }
            if (eventId.HasValue)
            {
                query = query.Where(s => s.School.SchoolEvents.Any(se => se.EventId == eventId.Value));
            }

            var result = await query.Select(s => new GetStudentBySchoolAndEvent
            {
                Id = s.Id,
                Email = s.Email,
                Fullname = s.Fullname,
                GraduateYear =s.GraduateYear,
                Phonenumber = s.Phonenumber,
                Status = s.Status,
                Classname = s.Classname,
                Schoolname = s.School.Name,
                Passcode = s.Player.Passcode != null ? s.Player.Passcode : "N/A",
            }).OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
        }
    }
}
