using NetDot;

namespace NetDotTests
{
    public class DotNotationDeserializationTests {
        public class Data
        {
            public Person Pessoa { get; set; } = new();
        }
        public class Person
        {
            public string Nome { get; set; } = "";
            public int Idade { get; set; }
        }
        public class Parent : Person
        {
            public Person[] Children { get; set; } = Array.Empty<Person>();
        }

        [Fact]
        public void CanDeserializeSimpleCass() {
            var pessoa = DotNotation.Deserialize<Person>("""
                nome=felipe
                idade=47
                """);
            Assert.NotNull(pessoa);
            Assert.Equal("felipe", pessoa.Nome);
            Assert.Equal(47, pessoa.Idade);
        }
        [Fact]
        public void CanDeserializeClassInsideAnother() {
            var data = DotNotation.Deserialize<Data>("""
                pessoa.nome=felipe
                pessoa.idade=47
                """);
            Assert.NotNull(data);
            Assert.Equal("felipe", data.Pessoa.Nome);
            Assert.Equal(47, data.Pessoa.Idade);
        }

        [Fact]
        public void CanDeserializeCassWithArray() {
            var pessoa = DotNotation.Deserialize<Parent>("""
                nome=felipe
                idade=47
                children[0].nome=Bernardo
                children[0].idade=9
                """);
            Assert.NotNull(pessoa);
            Assert.Equal("Bernardo", pessoa.Children[0].Nome);
            Assert.Equal(9, pessoa.Children[0].Idade);
        }
        public record PersonRecord(string Name, int Age);
        [Fact]
        public void CanDeserializeRecordClass() {
            var person = DotNotation.Deserialize<PersonRecord>("""
                name=felipe
                age=47
                """);
            Assert.NotNull(person);
            Assert.Equal("felipe", person.Name);
            Assert.Equal(47, person.Age);
        }

        [Fact]
        public void CanDeserializeMasterDetailWithManyRecords() {
            var master = DotNotation.Deserialize<Master>("""
                Name=Master Record
                details[0].Id=123
                details[0].Tag=teste
                """);
            Assert.NotNull(master);
        }
    }

    public class Master {
        public string Name { get; set; }
        public Detail[] Details { get; set; }
    }
    public class Detail {
        public int Id { get; set; } 
        public string Tag { get; set; }
    }

    /*public class PictureHttpUpload
    {
        public string Enable { get; set; }
        public UploadServer[] UploadServerList { get; set; } = Array.Empty<UploadServer>();
    }*/
    public class UploadServer
    {
        public string Address { get; set; }
    }

}