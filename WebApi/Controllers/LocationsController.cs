using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Data;
using WebApi.Extensions;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class LocationsController(LocationRepository repo) : ControllerBase
    {
        private readonly LocationRepository _repo = repo;

        [HttpGet]
        [UseApiKey]
        [ProducesResponseType(typeof(IEnumerable<LocationModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Returns a collection of all locations.")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _repo.GetAllAsync();
            return response.Succeded ? Ok(response.Result) : StatusCode((int)response.StatusCode!);
        }

        [HttpGet("{id}")]
        [UseApiKey]
        [ProducesResponseType(typeof(LocationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Returns a single location with the given Id.")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _repo.GetAsync(id);
            return response.Succeded ? Ok(response.Result) : StatusCode((int)response.StatusCode!);
        }

        [HttpPost]
        [UseApiKey]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Create a new location.")]
        public async Task<IActionResult> Add(AddLocationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _repo.AddAsync(dto);
            return response.Succeded ? Created() : StatusCode((int)response.StatusCode!);
        }

        [HttpPut]
        [UseApiKey]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Updates an existing location.")]
        public async Task<IActionResult> Update(EditLocationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _repo.UpdateAsync(dto);
            return response.Succeded ? Ok() : StatusCode((int)response.StatusCode!);
        }

        [HttpDelete("{id}")]
        [UseApiKey]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Deletes a location with the given Id.")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _repo.DeleteAsync(id);
            return response.Succeded ? Ok() : StatusCode((int)response.StatusCode!);
        }
    }
}
