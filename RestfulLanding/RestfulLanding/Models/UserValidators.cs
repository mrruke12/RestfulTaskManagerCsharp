using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RestfulLanding.Models {
    public static class UserValidators {
        public static bool ValidateEmail(ModelStateDictionary ModelState, string Email) {
            if (string.IsNullOrEmpty(Email) || !System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) {
                ModelState.AddModelError("Email", LocalizationManager.current["InvalidEmail"]);
            }
            return ModelState.IsValid;
        }

        public static bool ValidatePassword(ModelStateDictionary ModelState, string Password) {
            if (string.IsNullOrEmpty(Password)) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordRequired"]);
            }

            if (Password != null && Password.Length < 6) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordInsufficientLength"] + "6");
            }

            if (Password != null && Password.Length > 30) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordOverheapingLength"] + "30");
            }

            if (Password != null && !System.Text.RegularExpressions.Regex.IsMatch(Password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()]).+$")) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordIncorrect"]);
            }

            return ModelState.IsValid;
        }

        public static bool UserRegisterValidate(ModelStateDictionary ModelState, ref UserRegister user, ref UserModel existingUser) {
            ValidateEmail(ModelState, user.Email);

            ValidatePassword(ModelState, user.Password);

            if (string.IsNullOrEmpty(user.PasswordConfirm)) {
                ModelState.AddModelError("PasswordConfirm", LocalizationManager.current["PasswordConfirmRequired"]);
            }

            if (user.PasswordConfirm != null && user.PasswordConfirm != user.Password) {
                ModelState.AddModelError("PasswordConfirm", LocalizationManager.current["PasswordsDontMatch"]);
            }

            if (existingUser != null) {
                ModelState.AddModelError("Email", LocalizationManager.current["UserEmailAlreadyExist"]);
            }

            return ModelState.IsValid;
        }

        public static bool UserLoginValidator(ModelStateDictionary ModelState, ref UserLogin user) {
            ValidateEmail(ModelState, user.Email);

            if (string.IsNullOrEmpty(user.Password)) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordRequired"]);
            }

            if (user.Password != null && user.Password.Length < 6 || user.Password != null && user.Password.Length > 30) {
                ModelState.AddModelError("Password", LocalizationManager.current["InvalidPassword"]);
            }

            return ModelState.IsValid;
        }
    }
}
