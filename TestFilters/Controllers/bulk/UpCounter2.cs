namespace TestFilters.Controllers.bulk;

public class UpCounter2
{
    private Counter _counter;

    public UpCounter2(Counter counter)
    {
        _counter = counter;
    }

    public int Count() => _counter.Count();
}