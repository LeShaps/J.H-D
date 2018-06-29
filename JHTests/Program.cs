using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JHTests
{
    public class Program
    {
        [Fact]
        public void TestInfos()
        {
            string source = "{Test}:{id:40;name:notest,immature:caster:Image}";
            Assert.Null(J.H_D.Program.getInfos("{Test}:{id:40;name:notest,immature:caster:Image}", source, '\0'));
            Assert.Equal("40", J.H_D.Program.getInfos(":{id:", source, ';'));
            Assert.Equal("id:40;name:notest,immature:caster:Image", J.H_D.Program.getInfos("}:{", source, '}'));
            Assert.Equal("immature:caster:Image}", J.H_D.Program.getInfos("st,", source, '\0'));
        }

        [Fact]
        public void TestDirectory()
        {
            J.H_D.Program.checkDir("Tests/Testdirectory/depth1/error");
            Assert.True(Directory.Exists("Tests/Testdirectory/depth1/error"));
            J.H_D.Program.checkDir("Tests/Testdirectory/depth2/okay/");
            Assert.True(Directory.Exists("Tests/Testdirectory/depth2/okay"));
            J.H_D.Program.checkDir("/");
            Assert.True(Directory.Exists("/"));
            J.H_D.Program.checkDir("Tests/Testdirectory/depth3/.magna");
            Assert.True(Directory.Exists("Tests/Testdirectory/depth3/.magna"));
            J.H_D.Program.checkDir("Tests/Testdirectory/depth3/♫doss");
            Assert.True(Directory.Exists("Tests/Testdirectory/depth3/♫doss"));
        }

        [Fact]
        public void ArgsTets()
        {
            Assert.Equal("First test on time", J.H_D.Program.makeArgs(new string[] { "First", "test", "on", "time" }));
            Assert.Equal("Never trush a ♫", J.H_D.Program.makeArgs(new string[] { "Never", "trush", "a", "♫" }));
            Assert.Equal("Expect to find a null here", J.H_D.Program.makeArgs(new string[] { "Expect", "to", "find", "a", null, "null", "here" }));
            Assert.Null(J.H_D.Program.makeArgs(null));
            Assert.Null(J.H_D.Program.makeArgs(new string[] { null, null, null }));
        }

        [Fact]
        public void ReplaceTests()
        {

        }
    }
}
