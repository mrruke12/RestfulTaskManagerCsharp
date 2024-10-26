using Microsoft.AspNetCore.Identity;

namespace RestfulLanding.Models {
    public class UserModel : IdentityUser {
        public string Email { get; set; }
        public int Completed { get; set; }
        public int Total { get; set; }

        public ChangeAuthModel ToChangeEmailModel() {
            return new ChangeAuthModel {
                Email = Email,
                Password = ""
            };
        }

        public ChangeAuthModel ToChangePasswordModel() {
            return new ChangeAuthModel {
                Password = "",
                Email = ""
            };
        }
    }

    public class UserRegister : UserModel {
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public UserModel ToUser() {
            return new UserModel {
                Email = Email,
                UserName = Email
            };
        }
    }

    public class UserLogin : UserModel {
        public string Password { get; set; }
    }

    public class ConfirmPasswordModel {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Action { get; set; }
    }

    public class ChangeAuthModel {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
