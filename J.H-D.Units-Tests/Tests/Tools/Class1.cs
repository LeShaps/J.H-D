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
            const string FirstDirTest = "CurrentDir";
            const string SecondDirTest = "Currentdir/Utilities/Tests/Moving";
            // The test here will check the "CheckDir" function, with invalid arguments, complex arguments, ect..

            Tools.Utilities.CheckDir(FirstDirTest);
            Assert.True(Directory.Exists(FirstDirTest));
            Directory.Delete(FirstDirTest);

            Action act = () => Tools.Utilities.CheckDir(null);
            Assert.Throws<ArgumentNullException>(act);

            Tools.Utilities.CheckDir(SecondDirTest);
            Assert.True(Directory.Exists(SecondDirTest));
            Directory.Delete(SecondDirTest, true);

            Tools.Utilities.CheckDir("Currentdir/Utilities/Tests///Moving");
            Assert.True(Directory.Exists(SecondDirTest));
            Directory.Delete(SecondDirTest, true);
        }
    }
}
