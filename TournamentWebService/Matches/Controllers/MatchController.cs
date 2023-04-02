using System;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TournamentWebService.Matches.Models;
using TournamentWebService.Matches.Services;

namespace TournamentWebService.Matches.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class MatchController : BaseController
    {
        private readonly MatchMongoDBService _matchMongoDBService;
        public MatchController(MatchMongoDBService matchMongoDBService)
        {
            _matchMongoDBService = matchMongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMatches()
        {
            try
            {
                List<Match> matches = await _matchMongoDBService.GetAllAsync();
                if (matches.Count == 0) return BadRequest(new { error = "No se encontraron partidos" });
                return Ok(new { message = "Partidos encontrados", matches });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMatch([FromBody] Match match)
        {
            try
            {
                if (match.tournamentId == null || match.homeTeam == null || match.visitingTeam == null || match.date == null)
                {
                    return BadRequest(new {error = "La petición no tiene la información suficiente para crear un partido"});
                }
                await _matchMongoDBService.CreateAsync(match);
                return CreatedAtAction(nameof(GetAllMatches), new { id = match.Id }, new { message = "Partido creado", match });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(string id, [FromBody] Match match)
        {
            try
            {
                Match matchAux = await _matchMongoDBService.GetOneAsync(id);
                if (matchAux == null) return BadRequest(new { error = "Partido no encontrado" });
                await _matchMongoDBService.UpdateAsync(id, match);
                return Ok(new { message = "Partido actualizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(string id)
        {
            try
            {
                Match matchAux = await _matchMongoDBService.GetOneAsync(id);
                if (matchAux == null) return BadRequest(new { error = "Partido no encontrado" });
                await _matchMongoDBService.DeleteAsync(id);
                return Ok(new { message = "Partido eliminado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatch(string id)
        {
            try
            {
                Match match = await _matchMongoDBService.GetOneAsync(id);
                if (match == null) return BadRequest(new { error = "Partido no encontrado" });
                return Ok(new { message = "Partido obtenido", match });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
