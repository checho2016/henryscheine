using System;
using System.Linq;
using Core;
using Xunit;

namespace CoreTests
{
    public class ParseServiceTests
    {
        //SUT: Subject Under Test
        IParserService<string> _parserService;

        static readonly string[] inputTemplate = new string[] {
                "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"",
                "\"Prescott, Zeke\",\"542-51-6641\",\"21\",\"801-555-2134\",\"Opratory=2,PCP=1\"",
                "\"Goldstein, Bucky\",\"635-45-1254\",\"42\",\"435-555-1541\",\"Opratory=1,PCP=1\"",
                "\"Vox, Bono\",\"414-45-1475\",\"51\",\"801-555-2100\",\"Opratory=3,PCP=2\""
            };

        public ParseServiceTests()
        {
            _parserService = new ParserService<string>();
        }

        [Fact]
        public void returns_right_header_columns_amount()
        {
            var input = new string[] { inputTemplate[0] };

            var result = _parserService.ParseText(string.Join(Environment.NewLine,input));

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(5, result.ElementAt(0).Count());
            Assert.Equal("Patient Name", result.ElementAt(0).ElementAt(0));
            Assert.Equal("SSN", result.ElementAt(0).ElementAt(1));
            Assert.Equal("Age", result.ElementAt(0).ElementAt(2));
            Assert.Equal("Phone Number", result.ElementAt(0).ElementAt(3));
            Assert.Equal("Status", result.ElementAt(0).ElementAt(4));
        }

        [Fact]
        public void returns_right_rows_amount()
        {
            var result = _parserService.ParseText(string.Join(Environment.NewLine, inputTemplate));

            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void returns_right_row_columns_amount()
        {
            var input = new string[] {
                inputTemplate[0],
                inputTemplate[3]
            };

            var result = _parserService.ParseText(string.Join(Environment.NewLine, input));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(5, result.ElementAt(1).Count());
            Assert.Equal("Vox, Bono", result.ElementAt(1).ElementAt(0));
            Assert.Equal("414-45-1475", result.ElementAt(1).ElementAt(1));
            Assert.Equal("51", result.ElementAt(1).ElementAt(2));
            Assert.Equal("801-555-2100", result.ElementAt(1).ElementAt(3));
            Assert.Equal("Opratory=3,PCP=2", result.ElementAt(1).ElementAt(4));
        }

        [Fact]
        public void returns_warning_when_input_is_null()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _parserService.ParseText(null));
            Assert.Equal("Input text can't be null", exception.Message);
        }

        [Fact]
        public void returns_warning_when_input_is_empty()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _parserService.ParseText(""));
            Assert.Equal("Input text can't be empty", exception.Message);
        }

        [Fact]
        public void returns_warning_when_input_contains_only_whitespaces()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => _parserService.ParseText(" "));
            Assert.Equal("Input text can't be empty", exception.Message);
        }

        [Fact]
        public void returns_empty_values_when_row_missing_columns()
        {
            var input = new string[] {
                inputTemplate[0],
                "\"Prescott, Zeke\",\"542-51-6641\",\"21\",\"801-555-2134\""
            };

            var result = _parserService.ParseText(string.Join(Environment.NewLine, input));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Equal(5, result.ElementAt(0).Count());
            Assert.Equal("Patient Name", result.ElementAt(0).ElementAt(0));
            Assert.Equal("SSN", result.ElementAt(0).ElementAt(1));
            Assert.Equal("Age", result.ElementAt(0).ElementAt(2));
            Assert.Equal("Phone Number", result.ElementAt(0).ElementAt(3));
            Assert.Equal("Status", result.ElementAt(0).ElementAt(4));

            Assert.Equal(5, result.ElementAt(1).Count());
            Assert.Equal("Prescott, Zeke", result.ElementAt(1).ElementAt(0));
            Assert.Equal("542-51-6641", result.ElementAt(1).ElementAt(1));
            Assert.Equal("21", result.ElementAt(1).ElementAt(2));
            Assert.Equal("801-555-2134", result.ElementAt(1).ElementAt(3));
            Assert.Equal("", result.ElementAt(1).ElementAt(4));
        }

        [Fact]
        public void ignores_additional_values_when_row_has_extra_columns()
        {
            var input = new string[] {
                inputTemplate[0],
                "\"Prescott, Zeke\",\"542-51-6641\",\"21\",\"801-555-2134\",\"Opratory=2,PCP=1\",\"some extra column\""
            };

            var result = _parserService.ParseText(string.Join(Environment.NewLine, input));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Equal(5, result.ElementAt(0).Count());
            Assert.Equal("Patient Name", result.ElementAt(0).ElementAt(0));
            Assert.Equal("SSN", result.ElementAt(0).ElementAt(1));
            Assert.Equal("Age", result.ElementAt(0).ElementAt(2));
            Assert.Equal("Phone Number", result.ElementAt(0).ElementAt(3));
            Assert.Equal("Status", result.ElementAt(0).ElementAt(4));

            Assert.Equal(5, result.ElementAt(1).Count());
            Assert.Equal("Prescott, Zeke", result.ElementAt(1).ElementAt(0));
            Assert.Equal("542-51-6641", result.ElementAt(1).ElementAt(1));
            Assert.Equal("21", result.ElementAt(1).ElementAt(2));
            Assert.Equal("801-555-2134", result.ElementAt(1).ElementAt(3));
            Assert.Equal("Opratory=2,PCP=1", result.ElementAt(1).ElementAt(4));
        }

        [Fact]
        public void filters_control_characters()
        {
            var input = new string[] {
                inputTemplate[0],
                "\"Prescott, Zeke\",\"542-51-6641\",\"21\",\"801-\u0002555-2134\",\"Opratory=2,PCP=1\",\"some extra column\""
            };

            var result = _parserService.ParseText(string.Join(Environment.NewLine, input));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Equal(5, result.ElementAt(0).Count());
            Assert.Equal("Patient Name", result.ElementAt(0).ElementAt(0));
            Assert.Equal("SSN", result.ElementAt(0).ElementAt(1));
            Assert.Equal("Age", result.ElementAt(0).ElementAt(2));
            Assert.Equal("Phone Number", result.ElementAt(0).ElementAt(3));
            Assert.Equal("Status", result.ElementAt(0).ElementAt(4));

            Assert.Equal(5, result.ElementAt(1).Count());
            Assert.Equal("Prescott, Zeke", result.ElementAt(1).ElementAt(0));
            Assert.Equal("542-51-6641", result.ElementAt(1).ElementAt(1));
            Assert.Equal("21", result.ElementAt(1).ElementAt(2));
            Assert.Equal("801-555-2134", result.ElementAt(1).ElementAt(3));
            Assert.Equal("Opratory=2,PCP=1", result.ElementAt(1).ElementAt(4));
        }

        [Fact]
        public void make_sure_generic_implementation_is_kept()
        {
            var parserService = new ParserService<int>();

            var input = new string[] {
                "\"20\",\"500\",\"21\",\"24\",\"98\"",
                "\"54\",\"635\",\"42\",\"435\",\"321\""
            };

            var result = parserService.ParseText(string.Join(Environment.NewLine, input));

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Equal(5, result.ElementAt(0).Count());
            Assert.Equal(20, result.ElementAt(0).ElementAt(0));
            Assert.Equal(500, result.ElementAt(0).ElementAt(1));
            Assert.Equal(21, result.ElementAt(0).ElementAt(2));
            Assert.Equal(24, result.ElementAt(0).ElementAt(3));
            Assert.Equal(98, result.ElementAt(0).ElementAt(4));

            Assert.Equal(5, result.ElementAt(1).Count());
            Assert.Equal(54, result.ElementAt(1).ElementAt(0));
            Assert.Equal(635, result.ElementAt(1).ElementAt(1));
            Assert.Equal(42, result.ElementAt(1).ElementAt(2));
            Assert.Equal(435, result.ElementAt(1).ElementAt(3));
            Assert.Equal(321, result.ElementAt(1).ElementAt(4));
        }
    }
}
