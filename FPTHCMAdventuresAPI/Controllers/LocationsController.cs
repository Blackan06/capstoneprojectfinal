using AutoMapper;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.MajorDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.MajorService;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.LocationServoce;
using DataAccess.Dtos.LocationDto;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.AnswerDto;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.ItemDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;

        public LocationsController(IMapper mapper, ILocationService locationService)
        {
            this._mapper = mapper;
            _locationService = locationService;
        }


        [HttpGet(Name = "GetLocationList")]

        public async Task<ActionResult<ServiceResponse<GetLocationDto>>> GetLocationList()
        {
            try
            {
                var res = await _locationService.GetLocation();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("location/pagination", Name = "GetLocationListWithPagination")]

        public async Task<ActionResult<ServiceResponse<LocationDto>>> GetLocationListWithPage([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var pagedsResult = await _locationService.GetLocationWithPage(queryParameters);
                return Ok(pagedsResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetLocationById(Guid id)
        {
            var eventDetail = await _locationService.GetLocationById(id);
            return Ok(eventDetail);
        }

        [HttpPost("location", Name = "Createlocation")]

        public async Task<ActionResult<ServiceResponse<GetLocationDto>>> CreateLocation(CreateLocationDto createLocationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _locationService.CreateNewLocation(createLocationDto);
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

        public async Task<ActionResult<ServiceResponse<GetLocationDto>>> UpdateLocation(Guid id, [FromBody] UpdateLocationDto updateLocationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _locationService.UpdateLocation(id, updateLocationDto);
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

        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            var serviceResponse = await _locationService.ImportDataFromExcel(file);

            if (serviceResponse.Success)
            {
                // Xử lý thành công
                return Ok(serviceResponse.Message);
            }
            else
            {
                // Xử lý lỗi
                return BadRequest(serviceResponse.Message);
            }
        }

        [HttpGet("excel-template")]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var serviceResponse = await _locationService.DownloadExcelTemplate();

            if (serviceResponse.Success)
            {
                // Trả về file Excel dưới dạng response
                return new FileContentResult(serviceResponse.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "SampleData.xlsx"
                };
            }
            else
            {
                // Xử lý lỗi nếu có
                return BadRequest(serviceResponse.Message);
            }
        }
        [HttpDelete("disablelocation")]
        public async Task<ActionResult<ServiceResponse<LocationDto>>> DisableStatusLocation(Guid id)
        {
            try
            {
                var disableEvent = await _locationService.DisableLocation(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}

