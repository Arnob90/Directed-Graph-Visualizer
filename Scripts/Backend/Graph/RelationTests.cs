using UltraUnitTesterSpace;
using UltraUnitTesterSpace.ResultTypesSpace;
using RelationParserSpace;
[TestClass]
class RelationTestClass
{
	[TestMethod]
    public static ResultType TestRelationParsing()
    {
        return new SuccessType();
    }
}
