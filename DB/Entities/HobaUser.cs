namespace HobaBackend.DB.Entities;

public class HobaUser
{
    /// <summary>
    /// Custom user id as per requirements
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Firebase user id
    /// </summary>
    public string Uid { get; set; } 

    /// <summary>
    /// Unique username
    /// </summary>
    public string Username { get; set; }
}