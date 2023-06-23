namespace TestFilters.Controllers;

public class Counter
{
    private int _count = 0;
    
    public int Count()
    {
        return _count++;
    }
    
}