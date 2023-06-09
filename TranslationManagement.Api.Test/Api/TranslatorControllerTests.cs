using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TranslationManagement.Api.Controllers;
using TranslationManagement.Api.DTO;
using TranslationManagement.Core.Interfaces;
using TranslationManagement.Core.Models;

namespace TranslationManagement.Tests.Api
{
    public class TranslatorControllerTests
    {
        private readonly Mock<ITranslatorRepository> _repo;
        private readonly TranslatorController _controller;

        public TranslatorControllerTests()
        {
            var _logger = new Mock<ILogger<TranslatorController>>();
            _repo = new Mock<ITranslatorRepository>();
            _controller = new TranslatorController(_repo.Object, _logger.Object);
        }

        [Fact]
        public async Task AddTranslatorReturnBadRequestForInvalidStatus()
        {
            _controller.ModelState.AddModelError("error", "some error");

            var t = new TranslatorCreate
            {
                Status = "uknkonwn",
                Name =  "test",
                HourlyRate= 10,
            };

            // Act
            var result = await _controller.AddTranslator(t);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsType<ActionResult<Translator>>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddTranslatorReturnOk()
        {
            var translator = new TranslatorCreate
            {
                Name = "test",
                HourlyRate = 10,
                Status = TranslatorStatus.Applicant,
            };

            // Act
            var result = await _controller.AddTranslator(translator);
            var createdResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.IsType<ActionResult<Translator>>(result);
            Assert.NotNull(createdResult);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task AddTranslatorCallRepositoryOnce()
        {
            Translator? t = null;
            _repo.Setup(r => r.AddAsync(It.IsAny<Translator>()))
                .Callback<Translator>(x => t = x);
            var translator = new TranslatorCreate
            {
                Name = "test",
                HourlyRate = 10,
                Status = TranslatorStatus.Applicant,
            };

            await _controller.AddTranslator(translator);

            // Assert
            _repo.Verify(x => x.AddAsync(It.IsAny<Translator>()), Times.Once);
            Assert.Equal(translator.Name, t.Name);
            Assert.Equal(translator.HourlyRate, t.HourlyRate);
        }
    }
}