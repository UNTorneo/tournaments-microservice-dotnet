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
        public TeamController(TeamMongoDBService teamMongoDBService, TournamentMongoDBService tournamentMongoDBService) {
            _teamMongoDBService = teamMongoDBService;
            _tournamentMongoDBService = tournamentMongoDBService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            try
            {
                List<Team> teams = await _teamMongoDBService.GetAllAsync();
                if (teams.Count == 0) return BadRequest(new { error = "No se encontraron equipos" });
                return Ok(new { message = "Equipos encontrados", teams });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            try
            {
                await _teamMongoDBService.CreateAsync(team);
                return CreatedAtAction(nameof(GetAllTeams), new { id = team.Id }, new { message = "Equipo creado", team });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(string id, [FromBody] Team team)
        {
            try
            {
                Team teamAux = await _teamMongoDBService.GetOneAsync(id);
                if (teamAux == null) return BadRequest(new { error = "No se encontraron equipos" });
                await _teamMongoDBService.UpdateAsync(id, team);
                return Ok(new { message = "Equipo actualizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            try
            {
                Team teamAux = await _teamMongoDBService.GetOneAsync(id);
                if (teamAux == null) return BadRequest(new { error = "No se encontraron equipos" });
                await _teamMongoDBService.DeleteAsync(id);
                return Ok(new { message = "Equipo eliminado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(string id)
        {
            try
            {
                Team team = await _teamMongoDBService.GetOneAsync(id);
                if (team == null) return BadRequest(new { error = "No se encontraron equipos" });
                return Ok(new { message = "Equipo encontrado", team });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch("tournament-register/{teamId}/{tournamentId}")]
        public async Task<IActionResult> RegisterTeam(string teamId, string tournamentId)
        {
            try
            {
                Team team = await _teamMongoDBService.GetOneAsync(teamId);
                if (team == null) return BadRequest(new { error = "Equipo no encontrado" });
                Tournament tournament = await _tournamentMongoDBService.GetOneAsync(tournamentId);
                if (tournament == null) return BadRequest(new { error = "Torneo no encontrado" });
                if (tournament.teams.Contains(teamId) || team.tournaments.Contains(tournamentId))
                    return BadRequest(new { error = "El equipo ya se encuentra registrado en este torneo" });
                team.tournaments.Add(tournamentId);
                await _teamMongoDBService.UpdateAsync(teamId, team);
                tournament.teams.Add(teamId);
                await _tournamentMongoDBService.UpdateAsync(tournamentId, tournament);
                return Ok(new { message = "Registro al torneo exitoso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
