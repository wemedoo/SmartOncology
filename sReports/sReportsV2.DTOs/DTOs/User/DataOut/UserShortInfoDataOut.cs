namespace sReportsV2.DTOs.User.DataOut
{
    public class UserShortInfoDataOut
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserShortInfoDataOut() { }

        public UserShortInfoDataOut(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string Name 
        { 
            get 
            {
                return $"{FirstName} {LastName}";
            } 
        }

    }
}