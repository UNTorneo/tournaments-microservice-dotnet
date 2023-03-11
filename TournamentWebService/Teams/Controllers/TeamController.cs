using System;
using System.Net;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TournamentWebService.Teams.Models;
using TournamentWebService.Teams.Services;
using TournamentWebService.Tournaments.Models;
using TournamentWebService.Tournaments.Services;

namespace TournamentWebService.Teams.Controllers
{
    [Controller]
    [Route("api/[Controller]")]
    public class TeamController : BaseController
    {
        private readonly TeamMongoDBService _teamMongoDBService;
        private readonly TournamentMongoDBService _tournamentMongoDBService;
        public TeamController(TeamMongoDBService teamMongoDBService) {
            _teamMongoDBService = teamMongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            try
            {
                List<Team> teams = await _teamMongoDBService.GetAllAsync();
                if (teams.Count == 0) { return CustomResult("No se encontraron equipos", HttpStatusCode.BadRequest); }
                return CustomResult(teams);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            try
            {
                await _teamMongoDBService.CreateAsync(team);
                return CreatedAtAction(nameof(GetAllTeams), new { id = team.Id }, team);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(string id, [FromBody] Team team)
        {
            try
            {
                Team teamAux = await _teamMongoDBService.GetOneAsync(id);
                if (teamAux == null){ return CustomResult("Equipo no encontrado", HttpStatusCode.BadRequest); }
                await _teamMongoDBService.UpdateAsync(id, team);
                return CustomResult("Equipo actualizado");
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            try
            {
                Team teamAux = await _teamMongoDBService.GetOneAsync(id);
                if (teamAux == null) { return CustomResult("Equipo no encontrado", HttpStatusCode.BadRequest); }
                await _teamMongoDBService.DeleteAsync(id);
                return CustomResult("Equipo eliminado");
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(string id)
        {
            try
            {
                Team team = await _teamMongoDBService.GetOneAsync(id);
                if (team == null) { return CustomResult("Equipo no encontrado", HttpStatusCode.BadRequest); }
                return CustomResult(team);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }

        [HttpPatch("tournament-register/{teamId}/{tournamentId}")]
        public async Task<IActionResult> RegisterTeam(string teamId, string tournamentId)
        {
            try
            {
                Team team = await _teamMongoDBService.GetOneAsync(teamId);
                if (team == null) { return CustomResult("Equipo no encontrado", HttpStatusCode.BadRequest); }
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(tournamentId);
                if (tournament == null) { return CustomResult("Torneo no encontrado", HttpStatusCode.BadRequest); }
                if (tournament.teams.Contains(teamId) || team.tournaments.Contains(tournamentId))
                    return CustomResult("El equipo ya se encuentra registrado en este torneo", HttpStatusCode.BadRequest);
                team.tournaments.Add(tournamentId);
                await _teamMongoDBService.UpdateAsync(teamId, team);
                tournament.teams.Add(teamId);
                await _tournamentMongoDBService.UpdateAsync(tournamentId, tournament);
                return CustomResult("Registro al torneo exitoso");
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadGateway);
            }
        }
    }
}
