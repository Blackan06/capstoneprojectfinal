using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.MajorDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.MajorService
{
    public interface IMajorService
    {
        Task<ServiceResponse<IEnumerable<GetMajorDto>>> GetMajor();
        Task<ServiceResponse<MajorDto>> GetMajorById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewMajor(CreateMajorDto createMajorDto);
        Task<ServiceResponse<bool>> UpdateMajor(Guid id, UpdateMajorDto majorDto);
        Task<string> getMajorName(Guid id); 
        Task<ServiceResponse<byte[]>> DownloadExcelTemplate();
        Task<ServiceResponse<string>> ImportDataFromExcel(IFormFile file);

        Task<ServiceResponse<bool>> DisableMajor(Guid id);

    }
}
