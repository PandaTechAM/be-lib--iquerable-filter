namespace EFCoreQueryMagic.Test.FilterTests;

public interface ITypedTests<in T>
{
    void TestEqual(T value);
    void TestNotEqual(T value);
    void TestGreaterThan(T value);
    void TestGreaterThanOrEqual(T value);
    void TestLessThan(T value);
    void TestLessThanOrEqual(T value);
    void TestContains(T value);
    void TestStartsWith(T value);
    void TestEndsWith(T value);
    void TestIn(T value);
    void TestNotIn(T value);
    void TestIsNotEmpty(T value);
    void TestIsEmpty(T value);
    void TestBetween(T value);
    void TestNotContains(T value);
    void TestHasCountEqualTo(T value);
    void TestHasCountBetween(T value);
    void TestIsTrue(T value);
    void TestIsFalse(T value);
}