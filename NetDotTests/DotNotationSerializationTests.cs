using NetDot;

namespace NetDotTests
{
    public class DotNotationSerializationTests {
        [Fact]
        public void NullSerializesToEmptyString() {
            var text = DotNotation.Serialize(null);
            Assert.Equal("", text);
        }
        [Fact]
        public void CanSerializeSimpleProperties() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 });
            Assert.Equal("""
                Name=Felipe
                Age=47

                """, 
                text);
        }
        [Fact]
        public void CanSerializeSimpleNestedProperties() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47, Brother = new { Name = "Julio" } });
            Assert.Equal("""
                Name=Felipe
                Age=47
                Brother.Name=Julio

                """,
                text);
        }
        [Fact]
        public void CanSerializeArrayWithValueTypes() {
            var text = DotNotation.Serialize(new { 
                Items = new object[] { "Felipe", 47 }
            });
            Assert.Equal("""
                Items[0]=Felipe
                Items[1]=47

                """,
                text);
        }
        [Fact]
        public void CanSerializeGenericDictionaryWithValueTypes() {
            var text = DotNotation.Serialize(new {
                Items = new Dictionary<string, object> { 
                    ["Name"] = "Felipe", 
                    ["Age"] =47 
                }
            });
            Assert.Equal("""
                Items[Name]=Felipe
                Items[Age]=47

                """,
                text);
        }
        [Fact]
        public void CanSerializeArraysWithinArraysWithValueTypes() {
            var text = DotNotation.Serialize(new {
                Items = new object[] { "Felipe", 47, new object[] { "Julio", 2 } }
            });
            Assert.Equal("""
                Items[0]=Felipe
                Items[1]=47
                Items[2][0]=Julio
                Items[2][1]=2

                """,
                text);
        }
        [Fact]
        public void CanSerializeDictionariesWithinArraysWithValueTypes() {
            var text = DotNotation.Serialize(new {
                Items = new object[] { "Felipe", 47, new Dictionary<string, object> { 
                    ["Name"]="Julio", 
                    ["Position"]=2 } 
                }
            });
            Assert.Equal("""
                Items[0]=Felipe
                Items[1]=47
                Items[2][Name]=Julio
                Items[2][Position]=2

                """,
                text);
        }
        [Fact]
        public void CanSerializeArraysWithinDictionaries() {
            var text = DotNotation.Serialize(new {
                dict = new Dictionary<string, object> {
                    ["Items"] = new object[] { "Felipe", 47 }
                }
            });
            Assert.Equal("""
                dict[Items][0]=Felipe
                dict[Items][1]=47

                """,
                text);
        }
        record Person(string Name, int Age);
        record Group(Person[] Persons);
        record Job(string Name, decimal rate);
        record Employee(
            string Name, int Age, 
            Group Friends, 
            Group ManagedPeople, 
            Group Supervisors,
            Job[] Jobs,
            Dictionary<string, Job> JobTransfers) : Person(Name, Age);

        [Fact]
        public void CanSerializeComplexObjectGraphs() {
            var person1 = new Person("Ricardo", 45);
            var person2 = new Person("Paulo", 72);
            var person3 = new Person("Marcelle", 52);
            var employee = new Employee(
                Name: "Felipe",
                Age: 47,
                Friends: new Group(new[] { person1, person3 }),
                ManagedPeople: new Group(new[] { person2 }),
                Supervisors: new Group(new[] { person3 }),
                Jobs: new[] { new Job("Worker", 20m), new Job("Slave", 1m) },
                JobTransfers: new() {
                    ["Night"] = new Job("Bouncer", 15m),
                }
                );
            var text = DotNotation.Serialize(employee);
            Assert.Equal("""
                Friends.Persons[0].Name=Ricardo
                Friends.Persons[0].Age=45
                Friends.Persons[1].Name=Marcelle
                Friends.Persons[1].Age=52
                ManagedPeople.Persons[0].Name=Paulo
                ManagedPeople.Persons[0].Age=72
                Supervisors.Persons[0].Name=Marcelle
                Supervisors.Persons[0].Age=52
                Jobs[0].Name=Worker
                Jobs[0].rate=20
                Jobs[1].Name=Slave
                Jobs[1].rate=1
                JobTransfers[Night].Name=Bouncer
                JobTransfers[Night].rate=15
                Name=Felipe
                Age=47

                """, text);
        }
        [Fact]
        public void CanChangeKeyValueSeparator() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 }, 
                settings: new DotNotationSettings { KeyValueSeparator = ":" });
            Assert.Equal("""
                Name:Felipe
                Age:47

                """,
                 text);
        }
        [Fact]
        public void CanAddSpaceBeforeValue() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 },
                settings: new DotNotationSettings { 
                    KeyValueSeparator = ":", 
                    SpacingBeforeValue = " " });
            Assert.Equal("""
                Name: Felipe
                Age: 47

                """,
                 text);
        }
        [Fact]
        public void CanAddSpaceAfterKey() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 },
                settings: new DotNotationSettings {
                    KeyValueSeparator = ":",
                    SpacingBeforeValue = " ",
                    SpacingAfterKey = " ",
                });
            Assert.Equal("""
                Name : Felipe
                Age : 47

                """,
                 text);
        }
        [Fact]
        public void CanSeparateItemsWithArbitrarySeparator() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 },
                settings: new DotNotationSettings { EntrySeparator = ", " });
            Assert.Equal("""
                Name=Felipe, Age=47, 
                """,
                 text);
        }
        [Fact]
        public void CanQuoteStrings() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 },
                settings: new DotNotationSettings { QuoteStrings = true });
            Assert.Equal("""
                Name="Felipe"
                Age=47

                """,
                 text);
        }
        [Fact]
        public void CanQuoteValues_WhichOverridesQuoteStrings() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 },
                settings: new DotNotationSettings { QuoteValues = true });
            Assert.Equal("""
                Name="Felipe"
                Age="47"

                """,
                 text);
        }
        [Fact]
        public void CanChangeQuoteChar() {
            var text = DotNotation.Serialize(new { Name = "Felipe", Age = 47 },
                settings: new DotNotationSettings { QuoteValues = true, QuoteChar = '_' });
            Assert.Equal("""
                Name=_Felipe_
                Age=_47_

                """,
                 text);
        }
        [Fact]
        public void CanChangeDotConnector() {
            var text = DotNotation.Serialize(new { Person = new { Name = "Felipe", Age = 47 } },
                settings: new DotNotationSettings { DotConnector = "_" });
            Assert.Equal("""
                Person_Name=Felipe
                Person_Age=47

                """,
                 text);
        }
        [Fact]
        public void CanTrimValues() {
            var text = DotNotation.Serialize(new { Person = new { Name = " Felipe   ", Age = 47 } },
                settings: new DotNotationSettings { TrimValues = true });
            Assert.Equal("""
                Person.Name=Felipe
                Person.Age=47

                """,
                 text);
        }
        [Fact]
        public void WithoutTrimmingSpacingIsPreserved() {
            var text = DotNotation.Serialize(new { Person = new { Name = " Felipe   ", Age = 47 } },
                settings: new DotNotationSettings { TrimValues = false, QuoteStrings = true });
            Assert.Equal("""
                Person.Name=" Felipe   "
                Person.Age=47

                """,
                 text);
        }
        [Fact]
        public void CanTrimMultipleCharacters() {
            var text = DotNotation.Serialize(new { Person = new { Name = " Felipe  ***  ", Age = 47 } },
                settings: new DotNotationSettings { TrimValues = true, TrimChars = new[] { ' ', '*' } });
            Assert.Equal("""
                Person.Name=Felipe
                Person.Age=47

                """,
                 text);
        }
        [Fact]
        public void CanSurroundEntriesWithOpeningAndClosingText() {
            var text = DotNotation.Serialize(new { Person = new { Name = "Felipe", Age = 47 } },
                settings: new DotNotationSettings { SurroundingTexts = ("{ ", " }")});
            Assert.Equal("""
                { Person.Name=Felipe }
                { Person.Age=47 }

                """,
                 text);
        }

    }
}