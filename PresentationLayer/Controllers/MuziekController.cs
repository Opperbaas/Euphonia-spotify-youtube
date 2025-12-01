using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;

namespace Euphonia.PresentationLayer.Controllers
{
    /// <summary>
    /// Controller voor Muziek API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MuziekController : ControllerBase
    {
        private readonly IMuziekService _service;

        public MuziekController(IMuziekService service)
        {
            _service = service;
        }

        // GET: api/muziek
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MuziekDto>>> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // GET: api/muziek/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MuziekDto>> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Muziek met ID {id} niet gevonden");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // GET: api/muziek/5/analyses
        [HttpGet("{id}/analyses")]
        public async Task<ActionResult<MuziekDto>> GetWithAnalyses(int id)
        {
            try
            {
                var item = await _service.GetWithAnalysesAsync(id);
                if (item == null)
                {
                    return NotFound($"Muziek met ID {id} niet gevonden");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // GET: api/muziek/search?term=beethoven
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MuziekDto>>> Search([FromQuery] string term)
        {
            try
            {
                var results = await _service.SearchAsync(term);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // GET: api/muziek/artiest/Mozart
        [HttpGet("artiest/{artiest}")]
        public async Task<ActionResult<IEnumerable<MuziekDto>>> GetByArtiest(string artiest)
        {
            try
            {
                var results = await _service.GetByArtiestAsync(artiest);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // POST: api/muziek
        [HttpPost]
        public async Task<ActionResult<MuziekDto>> Create([FromBody] CreateMuziekDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.MuziekID }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // PUT: api/muziek/5
        [HttpPut("{id}")]
        public async Task<ActionResult<MuziekDto>> Update(int id, [FromBody] UpdateMuziekDto dto)
        {
            try
            {
                if (id != dto.MuziekID)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _service.UpdateAsync(dto);
                if (updated == null)
                {
                    return NotFound($"Muziek met ID {id} niet gevonden");
                }

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // DELETE: api/muziek/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Muziek met ID {id} niet gevonden");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
