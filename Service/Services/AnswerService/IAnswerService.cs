using DataAccess;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.EventDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.AnswerService
{
    public interface IAnswerService
    {
        Task<ServiceResponse<IEnumerable<AnswerDto>>> GetAnswer();
        Task<ServiceResponse<IEnumerable<GetAnswerAndQuestionNameDto>>> GetListQuestionByMajorIdAsync(Guid majorId);       
        Task<ServiceResponse<PagedResult<AnswerDto>>> GetAnswerWithPage(QueryParameters queryParameters);
        Task<ServiceResponse<AnswerDto>> GetAnswerById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewAnswer(CreateAnswerDto createAnswerDto);
        Task<ServiceResponse<bool>> UpdateAnswer(Guid id, UpdateAnswerDto eventDto);
        Task<ServiceResponse<byte[]>> DownloadExcelTemplate();
        Task<ServiceResponse<string>> ImportDataFromExcel(IFormFile file);

    }
}
