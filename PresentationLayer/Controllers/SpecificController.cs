using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;

namespace Euphonia.PresentationLayer.Controllers
{
    /// <summary>
    /// API Controller - Presentation Layer
    /// Alleen verantwoordelijk voor HTTP requests/responses
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SpecificController : ControllerBase
    {
        private readonly ISpecificService _service;

        public SpecificController(ISpecificService service)
        {
            _service = service;
        }

        // GET: api/specific
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecificDto>>> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/specific/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecificDto>> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                
                if (item == null)
                {
                    return NotFound($"Item with Id {id} not found");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/specific/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SpecificDto>>> GetActive()
        {
            try
            {
                var items = await _service.GetActiveItemsAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/specific/search?name=test
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SpecificDto>>> Search([FromQuery] string name)
        {
            try
            {
                var items = await _service.SearchByNameAsync(name);
                return Ok(items);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/specific
        [HttpPost]
        public async Task<ActionResult<SpecificDto>> Create([FromBody] CreateSpecificDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/specific/5
        [HttpPut("{id}")]
        public async Task<ActionResult<SpecificDto>> Update(int id, [FromBody] UpdateSpecificDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest("Id mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _service.UpdateAsync(dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/specific/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                
                if (!result)
                {
                    return NotFound($"Item with Id {id} not found");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/specific/5/activate
        [HttpPost("{id}/activate")]
        public async Task<ActionResult> Activate(int id)
        {
            try
            {
                var result = await _service.ActivateAsync(id);
                
                if (!result)
                {
                    return NotFound($"Item with Id {id} not found");
                }

                return Ok("Item activated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/specific/5/deactivate
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(int id)
        {
            try
            {
                var result = await _service.DeactivateAsync(id);
                
                if (!result)
                {
                    return NotFound($"Item with Id {id} not found");
                }

                return Ok("Item deactivated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/specific/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetActiveCount()
        {
            try
            {
                var count = await _service.GetActiveCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
