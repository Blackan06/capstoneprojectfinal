using BusinessObjects.Model;
using DataAccess.Dtos.AnswerDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.AnswerRepositories
{
    public interface IAnswerRepository : IGenericRepository<Answer>
    {

        Task<IEnumerable<GetAnswerAndQuestionNameDto>> GetListQuestionByMajorIdAsync(Guid majorId);

    }
}
