using System.ComponentModel.DataAnnotations;

namespace Habr.BusinessLogic.Validation
{
    public static class UserValidation
    {
        public static bool IsValidEmail(string? email)
        {
            return email is {Length: <= 200} && new EmailAddressAttribute().IsValid(email);
        }

        public static bool IsValidPassword(string? password)
        {
            return password is {Length: >= 7 and <= 50} && 
                   password.ToLower() != password.ToUpper(); //Contains different cases
        }
    }
}
