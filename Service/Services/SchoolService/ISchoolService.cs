using BusinessObjects.Model;
using DataAccess;
using DataAccess.Dtos.SchoolDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.SchoolService
{
    public interface ISchoolService
    {
        Task<ServiceResponse<IEnumerable<GetSchoolDto>>> GetSchool();
        Task<ServiceResponse<SchoolDto>> GetSchoolById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewSchool(CreateSchoolDto createSchoolDto);
        Task<ServiceResponse<bool>> UpdateSchool(Guid id, UpdateSchoolDto schoolDto);
        Task<ServiceResponse<PagedResult<SchoolDto>>> GetSchoolWithPage(QueryParameters queryParameters);
        Task<ServiceResponse<bool>> DisableSchool(Guid id);
        Task<ServiceResponse<IEnumerable<School>>> GetSchoolByName(string schoolname);
    }
}
