using System;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TournamentWebService.Tournaments.Models;
using TournamentWebService.Tournaments.Services;

namespace TournamentWebService.Tournaments.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class TournamentController : BaseController
    {
        private readonly TournamentMongoDBService _tournamentMongoDBService;
        public TournamentController(TournamentMongoDBService tournamentMongoDBService)
        {
            _tournamentMongoDBService = tournamentMongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTournaments() {
            try
            {
                Console.WriteLine("GetAllTournaments");
                List<Tournament> tournaments = await _tournamentMongoDBService.GetAllAsync();
                if (tournaments.Count == 0) return BadRequest(new { error = "No se encontraron torneos" });
                return Ok(tournaments);
            }catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] Tournament tournament) {
            try
            {
                if (!TournamentDataValidation.tournamentAcces.Contains(tournament.access))
                {
                    return BadRequest(new { error = "El tipo de acceso del torneo no es valido" });
                }
                if (!TournamentDataValidation.tournamentStatus.Contains(tournament.status))
                {
                    return BadRequest(new { error = "El estado del torneo no es valido" });
                }
                await _tournamentMongoDBService.CreateAsync(tournament);
                return CreatedAtAction(nameof(GetAllTournaments), new { id = tournament.Id }, new { message = "Torneo creado", tournament });
            }catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTournament(string id, [FromBody] Tournament tournament) {
            try
            {
                Tournament tournamentAux = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournamentAux == null) return BadRequest(new { error = "Torneo no encontrado" });

                if (tournament.access != null && !TournamentDataValidation.tournamentAcces.Contains(tournament.access))
                {
                    return BadRequest(new { error = "El tipo de acceso del torneo no es valido" });
                }
                if (tournament.status != null && !TournamentDataValidation.tournamentStatus.Contains(tournament.status))
                {
                    return BadRequest(new { error = "El estado del torneo no es valido" });
                }
                await _tournamentMongoDBService.UpdateAsync(id, tournament);
                return Ok(new { message = "Torneo actualizado" });
            }catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(string id) {
            try
            {
                Tournament tournamentAux = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournamentAux == null) return BadRequest(new { error = "Torneo no encontrado" });
                await _tournamentMongoDBService.DeleteAsync(id);
                return Ok(new { message = "Torneo eliminado" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournament(string id)
        {
            try
            {
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournament == null) return BadRequest(new { error = "Torneo no encontrado" });
                return Ok(tournament);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("in-progress")]
        public async Task<IActionResult> GetActiveTournaments(string id)
        {
            try
            {
                List<Tournament> activeTournaments = await _tournamentMongoDBService.GetActiveTournamentsAsync();
                Console.WriteLine(activeTournaments);
                if (activeTournaments.Count() == 0) return BadRequest(new { error = "No hay torneos activos" });
                return Ok(activeTournaments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("by-status")]
        public async Task<IActionResult> GetTournamentsByStatus([FromQuery(Name = "status")] string status)
        {
            try
            {
                if (status != null && !TournamentDataValidation.tournamentStatus.Contains(status))
                {
                    return BadRequest(new { error = "El estado del torneo no es válido" });
                }
                List<Tournament> tournaments = await _tournamentMongoDBService.GetTournamentsByStatusAsync(status);
                if (tournaments.Count() == 0) return BadRequest(new { error = "No hay torneos con este estado" });
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
