using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http;
using System.Web.Mvc;
using TranslationManagement.Api.Controllers;
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

            var t = new Translator
            {
                Status = "uknkonwn"
            };

            // Act
            var result = await _controller.AddTranslator(t);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsType<ActionResult<Translator>>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(badRequestResult.StatusCode, StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task AddTranslatorReturnOk()
        {
            var t = new Translator
            {
                Name = "test",
                HourlyRate = 10,
            };

            // Act
            var result = await _controller.AddTranslator(t);
            var createdResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.IsType<ActionResult<Translator>>(result);
            Assert.NotNull(createdResult);
            Assert.Equal(createdResult.StatusCode, StatusCodes.Status201Created);
        }

        [Fact]
        public async Task AddTranslatorCallRepositoryOnce()
        {
            Translator? t = null;
            _repo.Setup(r => r.AddAsync(It.IsAny<Translator>()))
                .Callback<Translator>(x => t = x);
            var translator = new Translator
            {
                Name = "Test Employee",
                HourlyRate = 32,
            };
            await _controller.AddTranslator(translator);
            _repo.Verify(x => x.AddAsync(It.IsAny<Translator>()), Times.Once);
            Assert.Equal(t.Name, translator.Name);
            Assert.Equal(t.HourlyRate, translator.HourlyRate);
        }
    }
}