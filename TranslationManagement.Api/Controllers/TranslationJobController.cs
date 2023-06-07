using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using External.ThirdParty.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranslationManagement.Core.Models;
using TranslationManagement.Core.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;
using TranslationManagement.Core.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/jobs/")]
    public class TranslationJobController : ControllerBase
    {
        private const double pricePerCharacter = 0.01;

        private readonly ILogger<TranslationJobController> _logger;
        private readonly ITranslationJobRepository _jobRepository;
        private readonly ITranslatorRepository _translatorRepository;

        public TranslationJobController(
            ITranslationJobRepository jobRepository,
            ITranslatorRepository translatorRepository,
            ILogger<TranslationJobController> logger)
        {
            //scopeFactory.CreateScope().ServiceProvider.GetService<AppDbContext>();
            _jobRepository = jobRepository;
            _translatorRepository = translatorRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TranslationJob>>> GetJobs()
        {
            var jobs = await _jobRepository.ListAsync();

            return Ok(jobs);
        }

        [HttpGet("by-translator/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TranslationJob>>> GetJobsByTranslator(int id)
        {
            var jobs = await _jobRepository.GetByTranslatorAsync(id);

            return Ok(jobs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> GetJob(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> CreateJob(TranslationJob job, int translatorId)
        {
            return await CreateJobInternal(translatorId, job.CustomerName, job.OriginalContent);

        }

        [HttpPost("file")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> CreateJobWithFile(IFormFile file, int translatorId, string? customerName)
        {
            var reader = new StreamReader(file.OpenReadStream());
            string content;

            if (file.FileName.EndsWith(".txt"))
            {
                if (customerName == null) 
                {
                    return BadRequest("customerName empty");
                }
                content = await reader.ReadToEndAsync();
            }
            else if (file.FileName.EndsWith(".xml"))
            {
                //TODO: Add XML schema validation
                var xdoc = XDocument.Parse(await reader.ReadToEndAsync());
                content = xdoc.Root.Element("Content").Value;
                customerName = xdoc.Root.Element("Customer").Value.Trim();
            }
            else
            {
                return BadRequest("unsupported file type");
            }

            return await CreateJobInternal(translatorId, customerName, content);
        }

        [HttpPost("{id}/update-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> UpdateJobStatus(int id, int translatorId, string newStatus = "")
        {
            _logger.LogInformation("Job status update request received: " + newStatus + " for job " + id.ToString() + " by translator " + translatorId.ToString());

            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            
            if (!StatusExists(newStatus))
            {
                return BadRequest("invalid status");
            }

            if (!job.ChangeStatus(newStatus))
            {
                return BadRequest("invalid status state change");
            }
            
            await _jobRepository.UpdateAsync(job);

            return NoContent();
        }

        private async Task<ActionResult<TranslationJob>> CreateJobInternal(int translatorId, string customerName, string originalContent)
        {
            var translator = await _translatorRepository.GetByIdAsync(translatorId);
            if (translator == null)
            {
                return NotFound("invalid translatorId");
            }

            var job = new TranslationJob(translator, customerName)
            {
                OriginalContent = originalContent,
                Status = TranslationJobStatus.New,
            };
            job.UpdatePriceByPricePerCharacter(pricePerCharacter);

            bool success = await _jobRepository.AddAsync(job) > 0;
            if (success)
            {
                var notificationSvc = new UnreliableNotificationService();
                bool sent = false;
                do
                {
                    try
                    {
                        sent = notificationSvc.SendNotification("Job created: " + job.Id).Result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("New job notification not sent, exception: " + ex.Message);
                    }

                }
                while (!sent);

                _logger.LogInformation("New job notification sent with status: " + sent.ToString());
            }

            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }

        private bool StatusExists(string status)
        {
            return typeof(TranslationJobStatus).GetFields().Count(prop => prop.Name == status) > 0;
        }
    }
}