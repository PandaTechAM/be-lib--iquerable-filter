namespace TestFilters.Controllers.bulk;

public class Counter
{
    private int _count = 0;
    
    public int Count()
    {
        return _count++;
    }
    
}