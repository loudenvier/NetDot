using NetDot;
using System.Globalization;

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
                Name=Felipe, Age=47
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
                """, text);
        }
        [Fact]
        public void CanSurroundEntriesWithOpeningAndClosingText() {
            var text = DotNotation.Serialize(new { Person = new { Name = "Felipe", Age = 47 } },
                settings: new DotNotationSettings { SurroundingTexts = ("{ ", " }")});
            Assert.Equal("""
                { Person.Name=Felipe }
                { Person.Age=47 }
                """, text);
        }
        [Fact]
        public void WillUrlEncodeValueIfNeeded() {
            var text = DotNotation.Serialize(new { Item = "Açúcar Mascavo" }, settings: new() {
                UrlEncode = true,
            });
            Assert.Equal("""
                Item=A%C3%A7%C3%BAcar%20Mascavo
                """, text);
        }
        [Fact]
        public void WillUrlEncodeKeyIfNeeded() {
            var text = DotNotation.Serialize(new { Açúcar = "Doce" }, settings: new() {
                UrlEncode = true,
            });
            Assert.Equal("""
                A%C3%A7%C3%BAcar=Doce
                """, text);
        }
        [Fact]
        public void WillUrlEncodeAddedQuotesIfNeeded() {
            var text = DotNotation.Serialize(new { Item = "Açúcar Mascavo" }, settings: new() {
                UrlEncode = true,
                QuoteStrings = true,
            });
            Assert.Equal("""
                Item=%22A%C3%A7%C3%BAcar%20Mascavo%22
                """, text);
        }
        [Fact]
        public void BooleansAreSerializedAsLowerCase() {
            var text = DotNotation.Serialize(new { Bool1 = false, Bool2 = true });
            Assert.Equal("""
                Bool1=false
                Bool2=true
                """, text);
        }
        [Fact]
        public void DateTimesAreSerializedWithDateFormatString() {
            var fmt = "yyyy'-'MM'-'dd HH':'mm':'ss";
            var dt = new DateTime(2023, 03, 31, 08, 30, 15);
            var text = DotNotation.Serialize(new { dt }, settings: new() { DateFormatString = fmt });
            Assert.Equal("""
                dt=2023-03-31 08:30:15
                """, text);
        }
        [Fact]
        public void DateTimesNullableAreSerializedWithDateFormatString() {
            var fmt = "yyyy'-'MM'-'dd HH':'mm':'ss";
            DateTime? dt = new DateTime(2023, 03, 31, 08, 30, 15);
            var text = DotNotation.Serialize(new { dt }, settings: new() { DateFormatString = fmt });
            Assert.Equal("""
                dt=2023-03-31 08:30:15
                """, text);
        }
        [Fact]
        public void DateTimeWillRespectCulture() {
            var fmt = "yyyy/MMMM/dd HH:mm:ss";
            var ptbr = CultureInfo.GetCultureInfo("pt-br");
            var dt = new DateTime(2023, 03, 31, 08, 30, 15);
            var text = DotNotation.Serialize(new { dt }, settings: new() { DateFormatString = fmt, Culture = ptbr });
            Assert.Equal("""
                dt=2023/março/31 08:30:15
                """, text);
        }

        [Fact]
        public void DateTimeOffsetsAreSerializedWithDateFormatString() {
            var fmt = "yyyy'-'MM'-'dd HH':'mm':'ss";
            var dt = new DateTimeOffset(2023, 03, 31, 08, 30, 15, TimeSpan.FromHours(-3));
            var text = DotNotation.Serialize(new { dt }, settings: new() { DateFormatString = fmt });
            Assert.Equal("""
                dt=2023-03-31 08:30:15
                """, text);
        }
        [Fact]
        public void FloatingPointNumbersUseInvariantCulture() {
            var text = DotNotation.Serialize(new { n = 1.55 });
            Assert.Equal("""
                n=1.55
                """, text);
        }
        [Fact]
        public void FloatingPointNumbersWillUseSelectedCulture() {
            var ptbr = CultureInfo.GetCultureInfo("pt-br");
            var text = DotNotation.Serialize(new { n = 1.55 }, settings: new() { Culture = ptbr });
            Assert.Equal("""
                n=1,55
                """, text);
        }
        [Fact]
        public void DecimalsUseInvariantCulture() {
            var text = DotNotation.Serialize(new { n = 1_999_999.55 });
            Assert.Equal("""
                n=1999999.55
                """, text);
        }
        [Fact]
        public void DecimalsWillUseSelectedCulture() {
            var ptBr = CultureInfo.GetCultureInfo("pt-br");
            var text = DotNotation.Serialize(new { n = 1_999_999.55 }, settings: new() { Culture = ptBr });
            Assert.Equal("""
                n=1999999,55
                """, text);
        }
        [Fact]
        public void CanSerializeAsURLQueryString() {
            var queryString = DotNotation.Serialize(new {
                page = 10,
                pageSize = 50,
                user = new { id = 1, }, 
                token = "my token/123"
            }, settings: new () {
                UrlEncode = true,
                EntrySeparator = "&",
            });
            Assert.Equal("page=10&pageSize=50&user.id=1&token=my%20token%2F123", queryString);
        }

    }
}