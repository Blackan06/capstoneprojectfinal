using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.MajorDto;
using DataAccess.Dtos.NPCDto;
using DataAccess.Repositories.MajorRepositories;
using DataAccess.Repositories.NPCRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.NpcService
{
    public class NpcService : INpcService
    {
        private readonly INpcRepository _npcRepository;
        private readonly IMapper _mapper;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public NpcService(INpcRepository npcRepository, IMapper mapper)
        {
            _npcRepository = npcRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewNpc(CreateNpcDto createNpcDto)
        {
            if (await _npcRepository.ExistsAsync(s => s.Name == createNpcDto.Name))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: NPC with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createNpcDto.Introduce = createNpcDto.Introduce.Trim();
            createNpcDto.Name = createNpcDto.Name.Trim();
            createNpcDto.Status = createNpcDto.Status.Trim();
            TimeZoneVietName(createNpcDto.CreatedAt);

            var createNpc = _mapper.Map<Npc>(createNpcDto);
            createNpc.Id = Guid.NewGuid();

            await _npcRepository.AddAsync(createNpc);

            return new ServiceResponse<Guid>
            {
                Data = createNpc.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }
        private void TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
        }
        public async Task<ServiceResponse<NpcDto>> GetNpcById(Guid eventId)
        {
            try
            {

                var eventDetail = await _npcRepository.GetAsync<NpcDto>(eventId);
               
                if (eventDetail == null)
                {

                    return new ServiceResponse<NpcDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<NpcDto>
                {
                    Data = eventDetail,
                    Message = "Successfully",
                    StatusCode = 200,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<ServiceResponse<IEnumerable<NpcDto>>> GetNpc()
        {
            var npcList = await _npcRepository.GetAllAsync<NpcDto>();

            if (npcList != null)
            {
                npcList = npcList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<NpcDto>>
                {
                    Data = npcList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<NpcDto>>
                {
                    Data = npcList,
                    Success = false,
                    Message = "Faile because List npc null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateNpc(Guid id, UpdateNpcDto updateNpcDto)
        {
            if (await _npcRepository.ExistsAsync(s => s.Name == updateNpcDto.Name))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: NPC with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingNpc = await _npcRepository.GetById(id);

            if (existingNpc == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Inventory not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            try
            {
                existingNpc.Introduce = updateNpcDto.Introduce.Trim();
                existingNpc.Name = updateNpcDto.Name.Trim();
                existingNpc.Status = updateNpcDto.Status.Trim();
                await _npcRepository.UpdateAsync(id, existingNpc);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success edit",
                    Success = true,
                    StatusCode = 202
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventTaskExists(id))
                {
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Message = "Invalid Record Id",
                        Success = false,
                        StatusCode = 500
                    };
                }
                else
                {
                    throw;
                }
            }
        }
        private async Task<bool> EventTaskExists(Guid id)
        {
            return await _npcRepository.Exists(id);
        }

        public async Task<ServiceResponse<NpcDto>> GetNpcByName(string npcName)
        {
            try
            {

                var npcDetail = await _npcRepository.GetNpcByName(npcName);

                if (npcDetail == null)
                {

                    return new ServiceResponse<NpcDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<NpcDto>
                {
                    Data = npcDetail,
                    Message = "Successfully",
                    StatusCode = 200,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public async Task<ServiceResponse<bool>> DisableNpc(Guid id)
        {
            var checkEvent = await _npcRepository.GetAsync<NpcDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                checkEvent.Status = "INACTIVE";
                var npcData = _mapper.Map<Npc>(checkEvent);

                await _npcRepository.UpdateAsync(id, npcData);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }
    }
}
