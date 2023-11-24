namespace RebacExperiments.Acl.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var a = File.ReadAllText("example_acl.txt");

            var result = NamespaceConfigurationParser.Parse(a);
        }
    }
}