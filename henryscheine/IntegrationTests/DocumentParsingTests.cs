using System;
using System.Linq;
using Api.Controllers;
using Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IntegrationTests
{
    public class DocumentParsingTests
    {
        //SUT: Subject Under Test
        DocumentParserController _controller;

        Mock<ILogger<DocumentParserController>> _logger = new Mock<ILogger<DocumentParserController>>();

        public DocumentParsingTests()
        {
            _controller = new DocumentParserController(
                new ParserService<string>(),
                _logger.Object);
        }

        [Fact]
        public void returns_object_from_parsed_text()
        {
            string[] input = new string[] {
                "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"",
                "\"Vox, Bono\",\"414-45-1475\",\"51\",\"801-555-2100\",\"Opratory=3,PCP=2\""
            };

            var result = _controller.Get(string.Join(Environment.NewLine, input));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Equal(5, result.ElementAt(0).Count());
            Assert.Equal("Patient Name", result.ElementAt(0).ElementAt(0));
            Assert.Equal("SSN", result.ElementAt(0).ElementAt(1));
            Assert.Equal("Age", result.ElementAt(0).ElementAt(2));
            Assert.Equal("Phone Number", result.ElementAt(0).ElementAt(3));
            Assert.Equal("Status", result.ElementAt(0).ElementAt(4));
            
            Assert.Equal(5, result.ElementAt(1).Count());
            Assert.Equal("Vox, Bono", result.ElementAt(1).ElementAt(0));
            Assert.Equal("414-45-1475", result.ElementAt(1).ElementAt(1));
            Assert.Equal("51", result.ElementAt(1).ElementAt(2));
            Assert.Equal("801-555-2100", result.ElementAt(1).ElementAt(3));
            Assert.Equal("Opratory=3,PCP=2", result.ElementAt(1).ElementAt(4));
        }

        [Fact]
        public void returns_null_on_exception()
        {
            var result = _controller.Get("");

            Assert.Null(result);
        }

        [Fact]
        public void logs_error_on_exception()
        {
            _logger.Setup(x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                )
            );

            _controller.Get("");

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
