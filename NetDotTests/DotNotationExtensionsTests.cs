using NetDot;

namespace NetDotTests
{
    public class DotNotationExtensionsTests {
        [Fact]
        public void AsQueryStringWorks() {
            var queryString = new {
                page = 10,
                pageSize = 50,
                user = new { id = 1, },
                token = "my token/123"
            }.AsQueryString();
            Assert.Equal("page=10&pageSize=50&user.id=1&token=my%20token%2F123", queryString);
        }
        [Fact]
        public void AsQueryStringDoesNotOverrideDefaultSettings() {
            var obj = new { id = "abc/123" };
            var qs = obj.AsQueryString();
            Assert.Equal("id=abc%2F123", qs);
            var text = DotNotation.Serialize(obj);
            Assert.Equal("id=abc/123", text);
        }
    }
}