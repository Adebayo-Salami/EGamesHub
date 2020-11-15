using System.ComponentModel.DataAnnotations;

namespace EGames.Models
{
    public class LoginRegisterViewModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayMessage { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string ReTypePassword { get; set; }
        public bool AgreeToLicense { get; set; }
    }
}
