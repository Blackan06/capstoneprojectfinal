using BusinessObjects.Model;
using DataAccess.Dtos.MajorDto;
using DataAccess.Dtos.NPCDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.MajorRepositories
{
    public interface  IMajorRepository : IGenericRepository<Major>
    {
        Task<Major> GetMajorByName(string majorName);

    }
}
