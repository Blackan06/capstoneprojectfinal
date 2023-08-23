using BusinessObjects.Model;
using DataAccess.Dtos.NPCDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.NPCRepository
{
    public interface INpcRepository : IGenericRepository<Npc>
    {
        Task<NpcDto> GetNpcByName(string npcName);


    }
}
