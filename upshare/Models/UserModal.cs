using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;

namespace upshare.Models
{
    public class UserModel : BaseModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        // Add other properties as needed

        // Required parameterless constructor
        public UserModel() { }
    }
}
    