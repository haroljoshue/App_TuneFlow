﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MVC.TUNEFLOW.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "Correo inválido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "¿Recordarme?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Limpia cualquier cookie de inicio de sesión externa
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Console.WriteLine("OnPostAsync iniciado");

            returnUrl ??= Url.Action("MainPage", "Home");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                Console.WriteLine($"Intentando login para: {Input.Email}");

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                Console.WriteLine($"Resultado login: Succeeded={result.Succeeded}, Requires2FA={result.RequiresTwoFactor}, IsLockedOut={result.IsLockedOut}");

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario inició sesión correctamente.");
                    Console.WriteLine("Login exitoso");

                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    if (user != null)
                    {
                        Console.WriteLine($"Usuario encontrado: {user.UserName}");

                        var roles = await _userManager.GetRolesAsync(user);
                        Console.WriteLine($"Roles del usuario: {string.Join(", ", roles)}");

                        if (roles.Contains("cliente"))
                        {
                            Console.WriteLine("entro a rol cliente");
                            return RedirectToAction("Panel", "Panel", new { area = "Cliente" });
                        }
                        else if (roles.Contains("artista"))
                        {
                            Console.WriteLine("entro a artista");
                            return RedirectToAction("Panel", "Panel", new { area = "Artista" });
                        }
                        else if (roles.Contains("admin"))
                        {
                            Console.WriteLine("entro a admin");
                            return RedirectToAction("Panel", "Panel", new { area = "Admin" });
                        }
                        else
                        {
                            Console.WriteLine("No se encontró un rol válido para el usuario");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Usuario no encontrado después de login exitoso");
                    }

                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    Console.WriteLine("Requiere autenticación de dos factores");
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Cuenta bloqueada por múltiples intentos fallidos.");
                    Console.WriteLine("Cuenta bloqueada");
                    return RedirectToPage("./Lockout");
                }

                Console.WriteLine("Intento de inicio de sesión inválido");
                ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido.");
            }
            else
            {
                Console.WriteLine("ModelState inválido");
            }

            return Page();
        }


    }
}
