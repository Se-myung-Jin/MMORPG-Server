using AccountServer;

public static class Extensions
{
    public static bool SaveChangesEx(this AppDbContext db)
    {
        try
        {
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}