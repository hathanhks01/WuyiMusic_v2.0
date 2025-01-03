using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WuyiMusic_DAL.DTOS;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_Services.IServices;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementSer;

        public AdvertisementController(IAdvertisementService advertisementSer)
        {
            _advertisementSer = advertisementSer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAllAdvertisements()
        {
            var advertisement = await _advertisementSer.GetAllAdvertisement();
            return Ok(advertisement);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisementsById(Guid id)
        {
            var advertisement = await _advertisementSer.GetByIdAdvertisement(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return Ok(advertisement);
        }

        [HttpPost]
        public async Task<ActionResult<Advertisement>> CreateAdvertisements(AdvertisementDto advertisementDto)
        {
            await _advertisementSer.AddAdvertisement(advertisementDto);
            return CreatedAtAction(nameof(GetAdvertisementsById), new { id = advertisementDto.AdvertisementId }, advertisementDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvertisements(Guid id, AdvertisementDto advertisementDto)
        {
            if (id != advertisementDto.AdvertisementId)
            {
                return BadRequest();
            }

            await _advertisementSer.UpdateAdvertisement(advertisementDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisements(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
