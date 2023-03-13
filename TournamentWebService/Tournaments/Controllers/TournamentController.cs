using System;
using System.Net;
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
                List<Tournament> tournaments = await _tournamentMongoDBService.GetAllAsync();
                if (tournaments.Count == 0) { return CustomResult("No se encontraron torneos", HttpStatusCode.BadRequest); }
                return CustomResult(tournaments);
            }catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] Tournament tournament) {
            try
            {
                if (!TournamentDataValidation.tournamentAcces.Contains(tournament.access))
                {
                    return CustomResult("El tipo de acceso del torneo no es valido", HttpStatusCode.BadRequest);
                }
                if (!TournamentDataValidation.tournamentStatus.Contains(tournament.status))
                {
                    return CustomResult("El estado del torneo no es valido", HttpStatusCode.BadRequest);
                }
                await _tournamentMongoDBService.CreateAsync(tournament);
                return CreatedAtAction(nameof(GetAllTournaments), new { id = tournament.Id }, tournament);
            }catch(Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTournament(string id, [FromBody] Tournament tournament) {
            try
            {
                Tournament tournamentAux = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournamentAux == null) { return CustomResult("Torneo no encontrado", HttpStatusCode.BadRequest); }

                if (tournament.access != null && !TournamentDataValidation.tournamentAcces.Contains(tournament.access))
                {
                    return CustomResult("El tipo de acceso del torneo no es válido", HttpStatusCode.BadRequest);
                }
                if (tournament.status != null && !TournamentDataValidation.tournamentStatus.Contains(tournament.status))
                {
                    return CustomResult("El estado del torneo no es válido", HttpStatusCode.BadRequest);
                }
                await _tournamentMongoDBService.UpdateAsync(id, tournament);
                return CustomResult("Torneo actualizado");
            }catch(Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(string id) {
            try
            {
                Tournament tournamentAux = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournamentAux == null) { return CustomResult("Torneo no encontrado", HttpStatusCode.BadRequest); }
                await _tournamentMongoDBService.DeleteAsync(id);
                return CustomResult("Torneo eliminado");
            }
            catch(Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournament(string id)
        {
            try
            {
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(id);
                if (tournament == null) { return CustomResult("Torneo no encontrado", HttpStatusCode.BadRequest); }
                return CustomResult(tournament);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("in-progress")]
        public async Task<IActionResult> GetActiveTournaments(string id)
        {
            try
            {
                List<Tournament> activeTournaments = await _tournamentMongoDBService.GetActiveTournamentsAsync();
                if (activeTournaments.Count() == 0) { return CustomResult("No hay torneos activos", HttpStatusCode.BadRequest); }
                return CustomResult(activeTournaments);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
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
                    return CustomResult("El estado del torneo no es válido", HttpStatusCode.BadRequest);
                }
                List<Tournament> activeTournaments = await _tournamentMongoDBService.GetTournamentsByStatusAsync(status);
                if (activeTournaments.Count() == 0) { return CustomResult("No hay torneos activos", HttpStatusCode.BadRequest); }
                return CustomResult(activeTournaments);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }
    }
}
