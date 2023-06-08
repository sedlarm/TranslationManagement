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
using TranslationManagement.Core.Services;
using System.IO.Pipes;
using TranslationManagement.Core.Models.DTO;

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

        [HttpGet("unassigned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TranslationJob>>> GetUnassignedJobs()
        {
            var jobs = await _jobRepository.GetByTranslatorAsync(0);

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
        public async Task<ActionResult<TranslationJob>> CreateJob(TranslationJob job, int translatorId = 0)
        {
            return await CreateJobInternal(translatorId, job);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> UpdateJob(int id, [FromBody]TranslationJobUpdate jobUpdate)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            
            //check if valid status is provided
            if (jobUpdate.Status != null && !StatusExists(jobUpdate.Status))
            {
                return BadRequest("invalid status: " + jobUpdate.Status);
            }

            if (jobUpdate.Status != null && !job.ChangeStatus(jobUpdate.Status))
            {
                return BadRequest("invalid status state change");
            }

            job.Price = jobUpdate.Price;
            job.CustomerName = jobUpdate.CustomerName;
            job.OriginalContent = jobUpdate.OriginalContent;
            job.TranslatedContent = jobUpdate.TranslatedContent;
            job.Price = jobUpdate.Price;

            await _jobRepository.UpdateAsync(job);

            return NoContent(); 
        }

        [HttpPost("file")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> CreateJobWithFile(IFormFile file, int translatorId = 0, string customerName = "")
        {
            TranslationJob job;

            if (file.FileName.EndsWith(".txt"))
            {
                if (string.IsNullOrEmpty(customerName)) 
                {
                    return BadRequest("customerName is empty");
                }
                //reads content from text file
                job = await TranslationJobFileReader.CreateTranslationJobFromStream(
                    TranslationJobFileReader.FileType.TXT, 
                    file.OpenReadStream()
                );
                job.CustomerName = customerName;
            }
            else if (file.FileName.EndsWith(".xml"))
            {
                //reads and parses job properties from XML file
                job = await TranslationJobFileReader.CreateTranslationJobFromStream(
                    TranslationJobFileReader.FileType.XML,
                    file.OpenReadStream()
                );
            }
            else
            {
                return BadRequest("unsupported file type");
            }

            return await CreateJobInternal(translatorId, job);
        }

        [HttpPost("{id}/update-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> UpdateJobStatus(int id, string newStatus = "")
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            
            //check if valid status is provided
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

        [HttpPost("{id}/assign-translator/{translatorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> AssignTranslator(int id, int translatorId)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            var translator = await _translatorRepository.GetByIdAsync(translatorId);
            if (translator == null)
            {
                return BadRequest("translator doesn't exists");
            }

            job.Translator = translator;

            await _jobRepository.UpdateAsync(job);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslationJob>> DeleteJob(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            await _jobRepository.DeleteAsync(job);

            return NoContent();
        }

        private async Task<ActionResult<TranslationJob>> CreateJobInternal(int translatorId, TranslationJob job)
        {
            if (translatorId > 0)
            {
                var translator = await _translatorRepository.GetByIdAsync(translatorId);
                if (translator == null)
                {
                    return NotFound("invalid translatorId");
                }
                job.Translator = translator;
            }

            if (!string.IsNullOrEmpty(job.Status) && !StatusExists(job.Status))
            {
                return BadRequest("invalid status");
            }

            job.UpdatePriceByPricePerCharacter(pricePerCharacter);

            bool success = await _jobRepository.AddAsync(job) > 0;
            if (success)
            {
                var notificationSvc = new UnreliableNotificationService();
                bool sent = false;
                while(!sent)
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
            }

            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }

        private static bool StatusExists(string status)
        {
            return typeof(TranslationJobStatus).GetFields().Count(prop => prop.Name == status) > 0;
        }
    }
}