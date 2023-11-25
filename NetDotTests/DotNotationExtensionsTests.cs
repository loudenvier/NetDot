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
    }
}