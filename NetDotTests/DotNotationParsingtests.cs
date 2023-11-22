using NetDot;
using NuGet.Frameworks;
using System.Dynamic;
using System.Reflection.Metadata;

namespace NetDotTests
{
    public class DotNotationParsingtests
    {
        [Fact]
        public void CanParseSimpleMember() {
            var dict = DotNotation.Parse("pessoa=felipe");
            Assert.NotNull(dict);
            Assert.Single(dict);
            Assert.Equal("felipe", dict["pessoa"]);
        }
        [Fact]
        public void CanParseSingleLine() {
            var dict = DotNotation.Parse("pessoa.nome=felipe");
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Single(pessoa);
            Assert.Equal("felipe", pessoa["nome"]);
        }
        [Fact]
        public void CanParseMultiLine() {
            var dict = DotNotation.Parse("""
                pessoa.nome=felipe
                pessoa.idade=47
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Equal(2, pessoa.Count);
            Assert.Equal("felipe", pessoa["nome"]);
            Assert.Equal("47", pessoa["idade"]);
        }
        [Fact]
        public void CanParseMultiLineWithManyLevels() {
            var dict = DotNotation.Parse("""
                pessoa.filho.nome=felipe
                pessoa.filho.esporte.preferido=judo
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Single(pessoa);
            var filho = pessoa["filho"] as Dictionary<string, object>;
            Assert.NotNull(filho);
            Assert.Equal(2, filho.Count);
            var esporte = filho["esporte"] as Dictionary<string, object>;
            Assert.NotNull(esporte);
            Assert.Single(esporte);
            Assert.Equal("judo", esporte["preferido"]);
        }
        [Fact]
        public void ParserWillReplaceRepeatedMember() {
            var dict = DotNotation.Parse("""
                pessoa.nome=felipe
                pessoa.idade=47
                pessoa.idade=48
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Equal(2, pessoa.Count);
            Assert.Equal("48", pessoa["idade"]);
        }
        [Fact]
        public void ArrayCanBeRootElement() {
            var dict = DotNotation.Parse("""
                pessoa[0]=felipe
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Single(pessoas);
            Assert.Equal("felipe", pessoas[0]);
        }
        [Fact]
        public void ArrayCanBeRootWithComplexElement() {
            var dict = DotNotation.Parse("""
                pessoa[0].nome=felipe
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Single(pessoas);
            var pessoa = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Single(pessoa);
            Assert.Equal("felipe", pessoa["nome"]);
        }
        [Fact]
        public void ArrayCanBeParsedWithSingleItem() {
            var dict = DotNotation.Parse("""
                pessoa.grupo[0].nome=felipe
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Single(pessoa);
            var grupos = pessoa["grupo"] as List<object?>;
            Assert.NotNull(grupos);
            Assert.Single(grupos);
            var grupo = grupos[0] as Dictionary<string, object>;
            Assert.NotNull(grupo);
            Assert.Equal("felipe", grupo["nome"]);
        }
        [Fact]
        public void ArrayCanBeParsedWithSimpleValue() {
            var dict = DotNotation.Parse("""
                pessoa.grupo[0]=grupozero
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoa = dict["pessoa"] as Dictionary<string, object>;
            Assert.NotNull(pessoa);
            Assert.Single(pessoa);
            var grupos = pessoa["grupo"] as List<object?>;
            Assert.NotNull(grupos);
            Assert.Single(grupos);
            Assert.Equal("grupozero", grupos[0]);
        }
        [Fact]
        public void ArrayInRootCanBeSetWithArbitraryIndex() {
            var dict = DotNotation.Parse("""
                pessoa[2]=felipe
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Equal(3, pessoas.Count);
            Assert.Equal("felipe", pessoas[2]);
        }
        [Fact]
        public void ArrayCanBeSetWithArbitraryIndex() {
            var dict = DotNotation.Parse("""
                time.pessoa[2]=felipe
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            Assert.Single(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Equal(3, pessoas.Count);
            Assert.Equal("felipe", pessoas[2]);
        }
        [Fact]
        public void ArrayCanBeParsedWithSingleElementWithTwoProperties() {
            var dict = DotNotation.Parse("""
                details[0].id=123
                details[0].tag=teste
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
        }
        [Fact]
        public void ArraysInRootCanHaveMultipleSimpleValuedItems() {
            var dict = DotNotation.Parse("""
                pessoa[2]=felipe
                pessoa[1]=julio
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Equal(3, pessoas.Count);
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
            Assert.Single(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            Assert.Single(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Equal(3, pessoas.Count);
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
            Assert.Single(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Single(pessoas);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Equal(2, felipe.Count);
            Assert.Equal("felipe", felipe["nome"]);
            var cursos = felipe["curso"] as List<object?>;
            Assert.NotNull(cursos);
            Assert.Single(cursos);
            Assert.Equal("judo", cursos[0]);
        }
        [Fact]
        public void ArraysCanHoldArrays() {
            var dict = DotNotation.Parse("""
                time.pessoa[0].curso[0]=judo
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Single(pessoas);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Single(felipe);
            var cursos = felipe["curso"] as List<object?>;
            Assert.NotNull(cursos);
            Assert.Single(cursos);
            Assert.Equal("judo", cursos[0]);
        }
        [Fact]
        public void ArrayItemInRootArrayCanBeComplex() {
            var dict = DotNotation.Parse("""
                pessoa[0].nome=felipe
                pessoa[0].idade=47
                """);
            Assert.NotNull(dict);
            Assert.Single(dict);
            var pessoas = dict["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Single(pessoas);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Equal(2, felipe.Count);
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
            Assert.Single(dict);
            var time = dict["time"] as Dictionary<string, object>;
            Assert.NotNull(time);
            Assert.Single(time);
            var pessoas = time["pessoa"] as List<object?>;
            Assert.NotNull(pessoas);
            Assert.Single(pessoas);
            var felipe = pessoas[0] as Dictionary<string, object>;
            Assert.NotNull(felipe);
            Assert.Equal(2, felipe.Count);
            Assert.Equal("felipe", felipe["nome"]);
            Assert.Equal("47", felipe["idade"]);
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
            Assert.Equal("Bernardo", pessoa.children[0]["nome"]);
            // since it's dynamic it will (for now) consider everything as strings
            Assert.Equal("9", pessoa.children[0]["idade"]);
            Assert.Same(pessoa, result);
        }
    }
}