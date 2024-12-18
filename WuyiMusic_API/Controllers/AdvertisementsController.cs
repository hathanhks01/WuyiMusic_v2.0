using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;

namespace WuyiMusic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly IAdvertisementRepository _advertisementRepository;

        public AdvertisementsController(IAdvertisementRepository advertisementRepository)
        {
            _advertisementRepository = advertisementRepository;
        }

        // GET: api/advertisement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAllAdvertisements()
        {
            var advertisements = await _advertisementRepository.GetAllAsync();
            return Ok(advertisements);
        }

        // GET: api/advertisement/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisementById(Guid id)
        {
            var advertisement = await _advertisementRepository.GetByIdAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return Ok(advertisement);
        }

        // POST: api/advertisement
        [HttpPost]
        public async Task<ActionResult<Advertisement>> CreateAdvertisement(Advertisement advertisement)
        {
            await _advertisementRepository.AddAsync(advertisement);
            return CreatedAtAction(nameof(GetAdvertisementById), new { id = advertisement.AdvertisementId }, advertisement);
        }

        // PUT: api/advertisement/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvertisement(Guid id, Advertisement advertisement)
        {
            if (id != advertisement.AdvertisementId)
            {
                return BadRequest();
            }

            await _advertisementRepository.UpdateAsync(advertisement);
            return NoContent();
        }

        // DELETE: api/advertisement/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisement(Guid id)
        {
            await _advertisementRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
