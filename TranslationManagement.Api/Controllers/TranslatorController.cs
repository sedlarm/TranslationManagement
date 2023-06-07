using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranslationManagement.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TranslationManagement.Core.Interfaces;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/translators/")]
    public class TranslatorController : ControllerBase
    {
        private readonly ILogger<TranslatorController> _logger;
        private readonly ITranslatorRepository _translatorRepository;

        public TranslatorController(ITranslatorRepository translatorRepository, ILogger<TranslatorController> logger)
        {
            //scopeFactory.CreateScope().ServiceProvider.GetService<AppDbContext>();
            _translatorRepository = translatorRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Translator>>> GetTranslators()
        {
            var translators = await _translatorRepository.ListAsync();

            return Ok(translators);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Translator>> GetTranslator(int id)
        {
            var translator = await _translatorRepository.GetByIdAsync(id);
            if (translator == null)
            {
                return NotFound();
            }

            return Ok(translator);
        }

        [HttpGet("search-by-name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Translator>>> GetTranslatorsByName(string name)
        {
            var translators = await _translatorRepository.GetByNameAsync(name);

            return Ok(translators);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Translator>> AddTranslator(Translator translator)
        {
            if(!StatusExists(translator.Status))
            {
                return BadRequest("invalid status");
            }

            await _translatorRepository.AddAsync(translator);

            return CreatedAtAction(nameof(GetTranslator), new { id = translator.Id }, translator);
        }
        
        [HttpPost("{id}/update-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Translator>> UpdateTranslatorStatus(int id, string newStatus)
        {
            _logger.LogInformation("User status update request: " + newStatus + " for user " + id.ToString());

            var translator = await _translatorRepository.GetByIdAsync(id);
            if (translator == null)
            {
                return NotFound();
            }

            if (!StatusExists(newStatus))
            {
                return BadRequest("invalid status");
            }

            translator.Status = newStatus;

            await _translatorRepository.UpdateAsync(translator);

            return NoContent();
        }

        private bool StatusExists(string status)
        {
            return typeof(TranslatorStatus).GetFields().Count(prop => prop.Name == status) > 0;
        }
    }
}