using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.SchoolService;
using DataAccess.Dtos.SchoolDto;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.ItemDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SchoolsController : ControllerBase
    {
        private readonly ISchoolService _schoolService;
        private readonly IMapper _mapper;

        public SchoolsController(IMapper mapper, ISchoolService schoolService)
        {
            this._mapper = mapper;
            _schoolService = schoolService;
        }


        [HttpGet(Name = "GetSchool")]

        public async Task<ActionResult<ServiceResponse<GetSchoolDto>>> GetSchoolList()
        {
            try
            {
                var res = await _schoolService.GetSchool();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
      
        [HttpGet("GetSchoolByName")]

        public async Task<ActionResult<ServiceResponse<BusinessObjects.Model.School>>> GetSchoolByName(string name)
        {
            try
            {
                var res = await _schoolService.GetSchoolByName(name);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("school/pagination", Name = "GetSchoolListWithPagination")]

        public async Task<ActionResult<ServiceResponse<SchoolDto>>> GetLocationListWithPage([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var pagedsResult = await _schoolService.GetSchoolWithPage(queryParameters);
                return Ok(pagedsResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SchoolDto>> GetSchoolById(Guid id)
        {
            var eventDetail = await _schoolService.GetSchoolById(id);
            return Ok(eventDetail);
        }

        [HttpPost("school", Name = "CreateNewSchool")]

        public async Task<ActionResult<ServiceResponse<GetSchoolDto>>> CreateNewSchool( CreateSchoolDto createSchoolDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _schoolService.CreateNewSchool(createSchoolDto);
                if (!res.Success)
                {
                    return BadRequest(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPut("{id}")]

        public async Task<ActionResult<ServiceResponse<GetSchoolDto>>> UpdateSchool(Guid id, [FromBody] UpdateSchoolDto updateSchoolDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _schoolService.UpdateSchool(id, updateSchoolDto);
                if (!res.Success)
                {
                    return BadRequest(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("disableschool")]
        public async Task<ActionResult<ServiceResponse<ItemDto>>> DisableStatusSchool(Guid id)
        {
            try
            {
                var disableEvent = await _schoolService.DisableSchool(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}
