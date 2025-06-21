using System;

namespace upshare.Models.User;

public class GetUser
{
    public string userId { get; set; }
    public string userName { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public string profilePictureUrl { get; set; }
    public DateTime dateJoined { get; set; }
    public string address { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public string phoneNumber { get; set; }

}
