using System;
using Api.Controllers;
using Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiTests
{
    public class DocumentParserControllerTests
    {
        //SUT: Subject Under Test
        DocumentParserController _controller;

        Mock<IParserService<string>> _parserService = new Mock<IParserService<string>>();
        Mock<ILogger<DocumentParserController>> _logger = new Mock<ILogger<DocumentParserController>>();


        public DocumentParserControllerTests()
        {
            _controller = new DocumentParserController(_parserService.Object, _logger.Object);
        }

        [Fact]
        public void get_method_calls_parse_text_function()
        {
            _parserService.Setup(x => x.ParseText("")).Returns(new string[][] { });

            _controller.Get("");
            _parserService.Verify(x => x.ParseText(""), Times.Once);
        }

        [Fact]
        public void get_method_calls_logger_function_when_exception_is_thrown()
        {
            _parserService.Setup(x => x.ParseText("")).Throws(new ArgumentException("Error parsing a request"));

            _logger.Setup(x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                )
            );

            _controller.Get("");
            _parserService.Verify(x => x.ParseText(""), Times.Once);

            _logger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once,
                "Error parsing a request"
            );
        }
    }
}
