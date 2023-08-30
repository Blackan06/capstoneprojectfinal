using BusinessObjects.Model;
using DataAccess.Dtos.StudentDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DataAccess.Repositories.StudentRepositories
{
    public interface IStudentRepositories : IGenericRepository<Student>
    {
        Task<IEnumerable<StudentDto>> GetStudentBySchoolId(Guid SchoolId);
        Task<IEnumerable<GetStudentBySchoolAndEvent>> GetStudentByEventSchoolId(Guid schoolEventId);
        Task<List<string>> GetExistingEmails();
        Task<List<string>> GetExistingPhoneNumbers();
        Task<IEnumerable<GetStudentWithPlayerDto>> GetStudentsBySchoolIdAsync(Guid schoolId);
        Task UpdateStudentStatusAsync(Guid studentId, string newStatus);

    }
}

