using NetDot;
using System.Dynamic;

namespace NetDotTests
{
    public class DotNotationParsingtests
    {
        [Fact]
        public void CanParseSimpleMember() {
            var dict = DotNotation.Parse("pessoa=felipe");
            Assert.NotNull(dict);
            Assert.Equal("felipe", dict["pessoa"]);
        }
        [Fact]
        public void CanParseSingleLine() {
            var dict = DotNotation.Parse("pessoa.nome=felipe");
            Assert.NotNull(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Equal("felipe", pessoa["nome"]);
        }
        [Fact]
        public void CanParseMultiLine() {
            var dict = DotNotation.Parse("""
                pessoa.nome=felipe
                pessoa.idade=47
                """);
            Assert.NotNull(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Equal("felipe", pessoa["nome"]);
            Assert.Equal("47", pessoa["idade"]);
        }
        [Fact]
        public void ParserWillReplaceRepeatedMember() {
            var dict = DotNotation.Parse("""
                pessoa.nome=felipe
                pessoa.idade=47
                pessoa.idade=48
                """);
            Assert.NotNull(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Equal("48", pessoa["idade"]);
        }
        [Fact]
        public void CanParseSingleItemArray() {
            var dict = DotNotation.Parse("""
                pessoa.grupo[0].nome=felipe
                """);
            Assert.NotNull(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            var grupos = pessoa["grupo"] as List<object?>;
            Assert.NotNull(grupos);
            Assert.True(grupos.Count == 1);
            var grupo = grupos[0] as Dictionary<string, object>;
            Assert.NotNull(grupo);
            Assert.Equal("felipe", grupo["nome"]);
        }
        [Fact]
        public void CanParseSimpleValueArray() {
            var dict = DotNotation.Parse("""
                pessoa.grupo[0]=grupozero
                """);
            Assert.NotNull(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            var grupos = pessoa["grupo"] as List<object?>;
            Assert.NotNull(grupos);
            Assert.True(grupos.Count == 1);
            Assert.Equal("grupozero", grupos[0]);
        }
        [Fact]
        public void ArrayCanBeRootElement() {
            var dict = DotNotation.Parse("""
                pessoa[0]=felipe
                """);
            Assert.NotNull(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 1);
            Assert.Equal("felipe", pessoas[0]);
        }
        [Fact]
        public void ArrayInRootCanBeSetWithArbitraryIndex() {
            var dict = DotNotation.Parse("""
                pessoa[2]=felipe
                """);
            Assert.NotNull(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 3);
            Assert.Equal("felipe", pessoas[2]);
        }
        [Fact]
        public void ArrayCanBeSetWithArbitraryIndex() {
            var dict = DotNotation.Parse("""
                time.pessoa[2]=felipe
                """);
            Assert.NotNull(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 3);
            Assert.Equal("felipe", pessoas[2]);
        }
        [Fact]
        public void ArraysInRootCanHaveMultipleItems() {
            var dict = DotNotation.Parse("""
                pessoa[2]=felipe
                pessoa[1]=julio
                """);
            Assert.NotNull(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 3);
            Assert.Equal("felipe", pessoas[2]);
            Assert.Equal("julio", pessoas[1]);
        }
        [Fact]
        public void ArraysCanHaveMultipleItems() {
            var dict = DotNotation.Parse("""
                time.pessoa[2]=felipe
                time.pessoa[1]=julio
                """);
            Assert.NotNull(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 3);
            Assert.Equal("felipe", pessoas[2]);
            Assert.Equal("julio", pessoas[1]);
        }
        [Fact]
        public void ArraysInRootCanHoldArrays() {
            var dict = DotNotation.Parse("""
                pessoa[0].curso[0]=judo
                pessoa[0].nome=felipe
                """);
            Assert.NotNull(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 1);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Equal("felipe", felipe["nome"]);
            var cursos = felipe["curso"] as List<object?>;
            Assert.NotNull(cursos);
            Assert.Equal("judo", cursos[0]);
        }
        [Fact]
        public void ArraysCanHoldArrays() {
            var dict = DotNotation.Parse("""
                time.pessoa[0].curso[0]=judo
                time.pessoa[0].nome=felipe
                """);
            Assert.NotNull(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 1);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Equal("felipe", felipe["nome"]);
            var cursos = felipe["curso"] as List<object?>;
            Assert.NotNull(cursos);
            Assert.Equal("judo", cursos[0]);
        }
        [Fact]
        public void ArrayItemInRootArrayCanBeComplex() {
            var dict = DotNotation.Parse("""
                pessoa[0].nome=felipe
                pessoa[0].idade=47
                """);
            Assert.NotNull(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 1);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Equal("felipe", felipe["nome"]);
            Assert.Equal("47", felipe["idade"]);
        }
        [Fact]
        public void ArrayItemCanBeComplex() {
            var dict = DotNotation.Parse("""
                time.pessoa[0].nome=felipe
                time.pessoa[0].idade=47
                """);
            Assert.NotNull(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.True(pessoas.Count == 1);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Equal("felipe", felipe["nome"]);
            Assert.Equal("47", felipe["idade"]);
        }

        public class Person
        {
            public string Nome { get; set; }
            public int Idade { get; set; }
        }
        public class DataRecord
        {
            public Person Pessoa { get; set; }
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
        public void CanDeserializeClassWithChildren() {
            var data = DotNotation.Deserialize<DataRecord>("""
                pessoa.nome=felipe
                pessoa.idade=47
                """);
            Assert.NotNull(data);
            Assert.Equal("felipe", data.Pessoa.Nome);
            Assert.Equal(47, data.Pessoa.Idade);
        }

        public class Parent : Person
        {
            public Person[] Children { get; set; }
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
        [Fact]
        public void CanParseIntoExpandoObject() {
            dynamic pessoa = new ExpandoObject();
            dynamic result = DotNotation.Parse("""
                nome=felipe
                idade=47
                children[0].nome=Bernardo
                children[0].idade=9
                """, pessoa);
            Assert.NotNull(pessoa);
            Assert.Equal("Bernardo", pessoa.children[0].nome);
            // since it's dynamic it will (for now) consider everything as strings
            Assert.Equal("9", pessoa.children[0].idade);
            Assert.Same(pessoa, result);
        }
    }
}