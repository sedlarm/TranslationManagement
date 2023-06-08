using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections;
using System.Text;
using TranslationManagement.Api.Controllers;
using TranslationManagement.Core.Interfaces;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Tests.Api
{
    public class TranslationJobContollerTests
    {
        private readonly Mock<ITranslationJobRepository> _repo;
        private readonly Mock<ITranslatorRepository> _repo2;
        private readonly TranslationJobController _controller;

        public TranslationJobContollerTests()
        {
            var _logger = new Mock<ILogger<TranslationJobController>>();
            _repo = new Mock<ITranslationJobRepository>();
            _repo2 = new Mock<ITranslatorRepository>();
            _controller = new TranslationJobController(_repo.Object, _repo2.Object, _logger.Object);
        }

        [Fact]
        public async Task AddTranslationJobReturnBadRequestForInvalidStatus()
        {
            _controller.ModelState.AddModelError("error", "some error");

            var job = new TranslationJob
            {
                Status = "uknkonwn"
            };

            //tells repository that translator object exists
            Translator t = new Translator { Name = "translator", Id = 1 };
            _repo2.Setup(r => r.GetByIdAsync(1)).Returns(Task.FromResult<Translator?>(t));

            // Act
            var result = await _controller.CreateJob(job, 1);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsType<ActionResult<TranslationJob>>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddTranslationJobFromXMLFileSuccess()
        {
            string xmlData = "<?xml version='1.0' encoding='utf-8'?><root><Content>Content123</Content><Customer>Customer123</Customer></root>";

            var stream = new MemoryStream(Encoding.ASCII.GetBytes(xmlData));
            IFormFile file = new FormFile(stream, 0, xmlData.Length, "content.xml", "content.xml");

            //tells repository that translator object exists
            Translator t = new Translator { Name = "translator", Id = 1 };
            _repo2.Setup(r => r.GetByIdAsync(1)).Returns(Task.FromResult<Translator?>(t));

            // Act
            var result = await _controller.CreateJobWithFile(file, 1, null);
            var createdResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.IsType<ActionResult<TranslationJob>>(result);
            Assert.NotNull(createdResult);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }
    }
}
