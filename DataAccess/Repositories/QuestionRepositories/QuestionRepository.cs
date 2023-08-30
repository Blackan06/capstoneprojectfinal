using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.QuestionDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.QuestionRepositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public QuestionRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Question> GetByMajorIdAsync(Guid majorId)
        {
            var question = await _dbContext.Questions.FirstOrDefaultAsync(x => x.MajorId == majorId && x.Answers.Any());
            if(question == null)
            {
                return null;
            }
            return question;
        }

        public async Task<IEnumerable<ListQuestionAndAnswer>> GetQuestionAndAnswersAsync()
        {
            var questionAndAnswers = await _dbContext.Questions
                .Include(x => x.Major)
                .Include(q => q.Answers)
                .ToListAsync();

            var orderedQuestionAndAnswers = questionAndAnswers
                .Select(question => new ListQuestionAndAnswer
                {
                    Id = question.Id,
                    MajorId = question.Major.Id,
                    MajorName = question.Major.Name,
                    Name = question.Name,
                    Status = question.Status,
                    Answers = question.Answers
                        .Select(answer => new GetAnswerListDto
                        {
                            Id = answer.Id,
                            AnswerName = answer.AnswerName,
                            IsRight = answer.IsRight
                        })
                        .OrderByDescending(x => x.CreatedAt)
                        .ToList()
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return orderedQuestionAndAnswers;
        }

        public async Task<Question> GetQuestionByNameAsync(string questionName)
        {
            var question = await _dbContext.Questions.FirstOrDefaultAsync(x => x.Name == questionName.Trim());
            if(question  == null)
            {
                return null;
            }
            return question;
        }
    }
}
