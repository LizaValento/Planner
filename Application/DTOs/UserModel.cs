using Domain.Entities;

namespace Application.DTOs
{
    public class UserModel
    {
        public UserModel() { }
        public UserModel(User User)
        {
            Id = User.Id;
            FirstName = User.FirstName;
            LastName = User.LastName;
            Nickname = User.Nickname;
            Password = User.Password;
            Role = User.Role;
            Email = User.Email;
            EventParticipants = User.Events?.Select(b => new EventParticipantModel(b)).ToList() ?? new List<EventParticipantModel>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public List<EventParticipantModel> EventParticipants { get; set; }
    }
}
