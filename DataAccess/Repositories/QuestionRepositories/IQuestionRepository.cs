using BusinessObjects.Model;
using DataAccess.Dtos.QuestionDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.QuestionRepositories
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<Question> GetByMajorIdAsync(Guid majorId);

        Task<IEnumerable<ListQuestionAndAnswer>> GetQuestionAndAnswersAsync();

        Task<Question> GetQuestionByNameAsync(string questionName);
    }
}
