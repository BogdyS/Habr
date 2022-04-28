using System.ComponentModel.DataAnnotations;

namespace Habr.BusinessLogic.Validation
{
    public static class EmailValidation
    {
        static public bool IsValidEmail(string email) 
        {
            return new EmailAddressAttribute().IsValid(email);
        }
    }
}
