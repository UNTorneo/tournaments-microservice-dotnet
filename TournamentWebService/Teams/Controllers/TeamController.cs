﻿using System;
using System.Net;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TournamentWebService.Core;
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
                return Ok(teams);
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
                Console.WriteLine("########################################");
                foreach (var member in team.members) {Console.Write(member + " ,");}
                Console.WriteLine();
                Console.WriteLine("########################################");
                if (team == null) return BadRequest(new { error = "No se encontraron equipos" });
                List<Task<HttpResponseMessage>> taskList = new();
                HttpClient client = new();
                foreach (string user in team.members) {
                    var response = client.GetAsync($"{UrlConstants.usersMS}/users/{user}");
                    taskList.Add(response);
                }
                var usersMsResponse = await Task.WhenAll(taskList);
                List<Users?> teamsPopulated = new();
                for (int i = 0; i < usersMsResponse.Length; i++) {
                    if (usersMsResponse[i].IsSuccessStatusCode)
                    {
                        teamsPopulated.Add(await usersMsResponse[i].Content.ReadFromJsonAsync<Users>());
                    }
                    else {
                        teamsPopulated.Add(null);
                    }
                }
                TeamMembersPopulated res = new TeamMembersPopulated(team);
                res.members = teamsPopulated;
                return Ok(res);
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

        [HttpPatch("register-member/{teamId}/{userId}")]
        public async Task<IActionResult> RegisterTeamMember(string teamId, string userId)
        {
            try
            {
                Team team = await _teamMongoDBService.GetOneAsync(teamId);
                if (team == null) return BadRequest(new { error = "Equipo no encontrado" });
                if (team.members.Contains(userId))
                    return BadRequest(new { error = "El usuario ya es miembro de este equipo" });
                team.members.Add(userId);
                await _teamMongoDBService.UpdateAsync(teamId, team);
                return Ok(new { message = "Miembro registrado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
