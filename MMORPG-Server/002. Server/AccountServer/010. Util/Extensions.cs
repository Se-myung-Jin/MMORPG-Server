using AccountServer;
using SharedDB;

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

    public static bool SaveChangesEx(this SharedDbContext db)
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