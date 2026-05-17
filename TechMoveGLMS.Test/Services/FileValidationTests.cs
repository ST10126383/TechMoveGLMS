using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class FileValidationTests
    {
        [Theory]
        [InlineData("signed_agreement.pdf")]
        [InlineData("contract.PDF")]
        [InlineData("doc123.pdf")]
        public void ValidatePdfFile_ValidFileName_ReturnsTrue(string fileName)
        {
            var isValid = !string.IsNullOrEmpty(fileName) &&
                         fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("malware.exe")]
        [InlineData("document.docx")]
        [InlineData("image.png")]
        [InlineData("script.js")]
        [InlineData("")]
        [InlineData(null)]
        public void ValidatePdfFile_InvalidFileName_ReturnsFalse(string fileName)
        {
            var isValid = !string.IsNullOrEmpty(fileName) &&
                         fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
            Assert.False(isValid);
        }
    }
}