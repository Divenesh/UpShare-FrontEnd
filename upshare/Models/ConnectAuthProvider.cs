using Supabase;

namespace upshare;

public static class Client
{
    private static Supabase.Client? _instance;
    
    public static Supabase.Client Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Supabase.Client(
                    "https://oymbbgtbqpywfngyivur.supabase.co",
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im95bWJiZ3RicXB5d2ZuZ3lpdnVyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkyMjY2OTksImV4cCI6MjA2NDgwMjY5OX0.RrXHBHwX_4Je1GFTWUhppBt0EZK-n4T4RZ0DsgJc1cI",
                    new SupabaseOptions { AutoRefreshToken = true });
            }
            return _instance;
        }
    }
}