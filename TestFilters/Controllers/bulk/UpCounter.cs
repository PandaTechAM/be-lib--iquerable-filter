namespace TestFilters.Controllers.bulk;

public class UpCounter
{
    private Counter _counter;

    public UpCounter(Counter counter)
    {
        _counter = counter;
    }

    public int Count() => _counter.Count();
}