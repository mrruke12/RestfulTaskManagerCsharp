using Microsoft.AspNetCore.Identity;

namespace RestfulLanding.Models {
    public class User : IdentityUser {
        public string Email { get; set; }
        public int Completed { get; set; }
        public int General { get; set; }

        public void Init() {
            this.General = 0;
            this.Completed = 0;
        }
    }

    public class UserRegister : User {
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }

    public class UserLogin : User {
        public string Password { get; set; }
    }
}
