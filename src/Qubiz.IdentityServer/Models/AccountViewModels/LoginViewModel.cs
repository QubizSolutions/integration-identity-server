using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Qubiz.IdentityServer.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool EnableLocalLogin;

        public bool IsExternalLoginOnly;

        public bool AllowRememberLogin;

        public bool RememberLogin;

        public string ReturnUrl;

        public string Username;

        public string ExternalLoginScheme;

    }
}
