using Discord;
using System;
using System.IO;
using Xunit;

namespace J.H_D.Units_Tests
{
    public class Class1
    {
        [Fact]
        public void CheckDirectory()
        {
            // The test here will check the "CheckDir" function, with invalid arguments, complex arguments, ect..
            Tools.Utilities.CheckDir("CurrentDir");
            Assert.True(Directory.Exists("CurrentDir"));
            Directory.Delete("CurrentDir");
            Tools.Utilities.CheckDir("CurrentDir/");
            Assert.True(Directory.Exists("CurrentDir"));
            Directory.Delete("CurrentDir");
            Action act = () => Tools.Utilities.CheckDir(null);
            Assert.Throws<ArgumentNullException>(act);
            Tools.Utilities.CheckDir("Currentdir/Utilities/Tests/Moving");
            Assert.True(Directory.Exists("Currentdir/Utilities/Tests/Moving"));
            Directory.Delete("Currentdir/Utilities/Tests/Moving", true);
            Tools.Utilities.CheckDir("Currentdir/Utilities/Tests///Moving");
            Assert.True(Directory.Exists("Currentdir/Utilities/Tests/Moving"));
            Directory.Delete("Currentdir/Utilities/Tests/Moving", true);
        }
    }
}
