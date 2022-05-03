using System.ComponentModel.DataAnnotations;

namespace Habr.BusinessLogic.Validation
{
    public static class EmailValidation
    {
        public static bool IsValidEmail(string email)
        {
            return email.Length <= 200 && new EmailAddressAttribute().IsValid(email);
        }
    }
}
