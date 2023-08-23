using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.AnswerDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.AnswerRepositories
{
    public class AnswerRepository : GenericRepository<Answer> , IAnswerRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public AnswerRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetAnswerAndQuestionNameDto>> GetListQuestionByMajorIdAsync(Guid majorId)
        {
            var major = await _dbContext.Set<Major>().FindAsync(majorId);

            if (major == null)
            {
                return null;
            }
            else
            {
                var questions = await _dbContext.Set<Question>()
                    .Where(q => q.MajorId == majorId)
                    .ToListAsync();

                var random = new Random();

                var questionDtos = questions
                    .OrderBy(q => random.Next())
                    .Take(5)
                    .Select(question =>
                    {
                        var answers = _dbContext.Set<Answer>()
                            .Where(a => a.QuestionId == question.Id)
                            .ToList();

                        var randomizedAnswers = answers
                            .OrderBy(a => random.Next())
                            .ToList();

                        var correctAnswer = randomizedAnswers.FirstOrDefault(a => a.IsRight);
                        var correctAnswerIndex = randomizedAnswers.IndexOf(correctAnswer);

                        var answerDtos = randomizedAnswers.Select(a => new GetAnswerListDto
                        {
                            Id = a.Id,
                            AnswerName = a.AnswerName,
                            IsRight = a.IsRight
                        }).ToList();

                        if (answerDtos.Count < 2)
                        {
                            return null; // Không đủ số lượng câu trả lời, không hiển thị câu hỏi
                        }

                        return new GetAnswerAndQuestionNameDto
                        {
                            QuestionName = question.Name,
                            AnswerDtos = answerDtos,
                            CorrectAnswerIndex = correctAnswerIndex
                        };
                    })
                    .Where(q => q != null) // Lọc bỏ những câu hỏi không có đủ câu trả lời
                    .ToList();

                return questionDtos;
            }
        }

    }
}
