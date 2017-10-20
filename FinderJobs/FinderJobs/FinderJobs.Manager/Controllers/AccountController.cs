using System.Globalization;
using FinderJobs.Manager.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FinderJobs.Manager.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private SignInHelper _helper;

        private SignInHelper SignInHelper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new SignInHelper(UserManager, AuthenticationManager);
                }
                return _helper;
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInHelper.PasswordSignIn(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case FinderJobs.Manager.Models.SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case FinderJobs.Manager.Models.SignInStatus.LockedOut:
                    return View("Lockout");
                case FinderJobs.Manager.Models.SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case FinderJobs.Manager.Models.SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginApi(string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                return Json(new { sucesso = false, mensagem = "Usuario ou senha inválidos" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var result = await SignInHelper.PasswordSignIn(Email, Password, false, shouldLockout: false);
                switch (result)
                {
                    case FinderJobs.Manager.Models.SignInStatus.Success:
                        var user = UserManager.FindByName(Email);                        
                        var roles = user.Roles != null && user.Roles.Count > 0 ? user.Roles.Aggregate((i, j) => i + "," + j) : "";

                        return Json(new { sucesso = true, roles }, JsonRequestBehavior.AllowGet);
                    default:
                        return Json(new { sucesso = false, mensagem = "Usuario ou senha inválidos" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //new Infra.Log.Gerar().LogErro(Email, ex.Message);
                return Json(new { sucesso = false, mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInHelper.HasBeenVerified())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInHelper.GetVerifiedUserIdAsync());
            if (user != null)
            {
                // To exercise the flow without actually sending codes, uncomment the following line
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInHelper.TwoFactorSignIn(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case FinderJobs.Manager.Models.SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case FinderJobs.Manager.Models.SignInStatus.LockedOut:
                    return View("Lockout");
                case FinderJobs.Manager.Models.SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> UsuarioDisponivelApi(string Email)
        {
            var user = await UserManager.FindByNameAsync(Email);
            if (user == null)                            
                return Json(new { sucesso = true }, JsonRequestBehavior.AllowGet);                        
            else
                return Json(new { sucesso = false }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterApi(string Email, string Password, string Url)
        {
            var user = new ApplicationUser { UserName = Email, Email = Email };
            var result = await UserManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                code = HttpUtility.UrlEncode(code);
                var callbackUrl = string.Concat(Url, "?email=", Email, "&code=", code, "&date=", DateTime.Now.ToString("ddMMyyyyHHmmss"));

                var emailTitulo = "Confirmação de cadastro";
                var emailConteudo = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"> <head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/> <title>FindeJobs</title> <style type=\"text/css\"> body{margin: 0; padding: 0; min-width: 100%!important;}.content{width: 100%; max-width: 600px;}@media only screen and (min-device-width: 601px){.content{width: 600px !important;}}.header{padding: 40px 30px 20px 30px;}.col425{width: 425px!important;}.subhead{font-size: 15px; color: #ffffff; font-family: sans-serif; letter-spacing: 10px;}.h1{font-size: 33px; line-height: 38px; font-weight: bold;}.h1, .h2, .bodycopy{color: #153643; font-family: sans-serif;}.innerpadding{padding: 30px 30px 30px 30px;}.borderbottom{border-bottom: 1px solid #f2eeed;}.h2{padding: 0 0 15px 0; font-size: 24px; line-height: 28px; font-weight: bold;}.bodycopy{font-size: 16px; line-height: 22px;}.button{text-align: center; font-size: 18px; font-family: sans-serif; font-weight: bold; padding: 0 30px 0 30px;}.button a{color: #ffffff; text-decoration: none;}@media only screen and (min-device-width: 601px){.content{width: 600px !important;}.col425{width: 425px!important;}.col380{width: 380px!important;}}img{height: auto;}.footer{padding: 20px 30px 15px 30px;}.footercopy{font-family: sans-serif; font-size: 14px; color: #ffffff;}.footercopy a{color: #ffffff; text-decoration: underline;}@media only screen and (max-width: 550px), screen and (max-device-width: 550px){body[yahoo] .buttonwrapper{background-color: transparent!important;}body[yahoo] .button a{background-color: #e05443; padding: 15px 15px 13px!important; display: block!important;}}body[yahoo] .unsubscribe{display: block; margin-top: 20px; padding: 10px 50px; background: #b5c5dd; border-radius: 5px; text-decoration: none!important; font-weight: bold;}body[yahoo] .hide{display: none!important;}</style> </head> <body yahoo bgcolor=\"#bce8f1\"><!--[if (gte mso 9)|(IE)]><table width=\"600\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td><![endif]--><table class=\"content\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td class=\"header\" bgcolor=\"#b5c5dd\"><table width=\"70\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td height=\"70\" style=\"padding: 0 20px 20px 0;\"><img src=\"http://finderjobs.com.br/content/img/profile.png\" width=\"70\" height=\"70\" border=\"0\" alt=\"\" / ></td></tr></table><!--[if (gte mso 9)|(IE)]><table width=\"425\" align=\"left\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td><![endif]--><table class=\"col425\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; max-width: 425px;\"><tr><td height=\"70\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"subhead\" style=\"padding: 0 0 0 3px;\">FinderJobs</td></tr><tr><td class=\"h1\" style=\"padding: 5px 0 0 0;\">Controle de Acesso</td></tr></table></td></tr></table><!--[if (gte mso 9)|(IE)]></td></tr></table><![endif]--></td></tr><tr> <td class=\"innerpadding borderbottom\"><table width=\"115\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"> <tr><td height=\"115\" style=\"padding: 0 20px 20px 0;\"> <img src=\"http://finderjobs.com.br/images/icone-acesso.png\" width=\"115\" height=\"115\" border=\"0\" alt=\"\"/></td></tr></table><!--[if (gte mso 9)|(IE)]> <table width=\"380\" align=\"left\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr> <td><![endif]--><table class=\"col380\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; max-width: 380px;\"> <tr><td> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr> <td class=\"bodycopy\">#mensagemTexto </td></tr><tr> <td style=\"padding: 20px 0 0 0;\"><table class=\"buttonwrapper\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr><td> <a href=\"#link\" class=\"unsubscribe\"> <font color=\"#000\">Clique aqui</font> </a></td></tr></table> </td></tr></table></td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr></table><!--[if (gte mso 9)|(IE)]></td></tr></table><![endif]--> </body></html>";
                emailConteudo = emailConteudo.Replace("#mensagemTexto", "Por favor, confirme seu cadastro clicando no botão abaixo e cadastre a sua senha");
                emailConteudo = emailConteudo.Replace("#link", callbackUrl);
                UserManager.SendEmailAsync(user.Id, emailTitulo, emailConteudo);
                
                return Json(new { sucesso = true, mensagem = "Foi enviado um email de confirmação para " + Email }, JsonRequestBehavior.AllowGet);
            }
            AddErrors(result);

            return Json(new { sucesso = false, mensagem = "Usuario ou senha inválidos" }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
                
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPasswordApi(string Email, string Url)
        {
            var user = await UserManager.FindByNameAsync(Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Json(new { sucesso = true, mensagem = "Enviado email de recuperação de senha" }, JsonRequestBehavior.AllowGet);
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            code = HttpUtility.UrlEncode(code);
            var callbackUrl = string.Concat(Url, "?email=", Email, "&code=", code, "&date=", DateTime.Now.ToString("ddMMyyyyHHmmss"));

            var emailTitulo = "Recuperação de Senha";
            var emailConteudo = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"> <head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/> <title>FindeJobs</title> <style type=\"text/css\"> body{margin: 0; padding: 0; min-width: 100%!important;}.content{width: 100%; max-width: 600px;}@media only screen and (min-device-width: 601px){.content{width: 600px !important;}}.header{padding: 40px 30px 20px 30px;}.col425{width: 425px!important;}.subhead{font-size: 15px; color: #ffffff; font-family: sans-serif; letter-spacing: 10px;}.h1{font-size: 33px; line-height: 38px; font-weight: bold;}.h1, .h2, .bodycopy{color: #153643; font-family: sans-serif;}.innerpadding{padding: 30px 30px 30px 30px;}.borderbottom{border-bottom: 1px solid #f2eeed;}.h2{padding: 0 0 15px 0; font-size: 24px; line-height: 28px; font-weight: bold;}.bodycopy{font-size: 16px; line-height: 22px;}.button{text-align: center; font-size: 18px; font-family: sans-serif; font-weight: bold; padding: 0 30px 0 30px;}.button a{color: #ffffff; text-decoration: none;}@media only screen and (min-device-width: 601px){.content{width: 600px !important;}.col425{width: 425px!important;}.col380{width: 380px!important;}}img{height: auto;}.footer{padding: 20px 30px 15px 30px;}.footercopy{font-family: sans-serif; font-size: 14px; color: #ffffff;}.footercopy a{color: #ffffff; text-decoration: underline;}@media only screen and (max-width: 550px), screen and (max-device-width: 550px){body[yahoo] .buttonwrapper{background-color: transparent!important;}body[yahoo] .button a{background-color: #e05443; padding: 15px 15px 13px!important; display: block!important;}}body[yahoo] .unsubscribe{display: block; margin-top: 20px; padding: 10px 50px; background: #b5c5dd; border-radius: 5px; text-decoration: none!important; font-weight: bold;}body[yahoo] .hide{display: none!important;}</style> </head> <body yahoo bgcolor=\"#bce8f1\"><!--[if (gte mso 9)|(IE)]><table width=\"600\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td><![endif]--><table class=\"content\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td class=\"header\" bgcolor=\"#b5c5dd\"><table width=\"70\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td height=\"70\" style=\"padding: 0 20px 20px 0;\"><img src=\"http://finderjobs.com.br/content/img/profile.png\" width=\"70\" height=\"70\" border=\"0\" alt=\"\" / ></td></tr></table><!--[if (gte mso 9)|(IE)]><table width=\"425\" align=\"left\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td><![endif]--><table class=\"col425\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; max-width: 425px;\"><tr><td height=\"70\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"subhead\" style=\"padding: 0 0 0 3px;\">FinderJobs</td></tr><tr><td class=\"h1\" style=\"padding: 5px 0 0 0;\">Controle de Acesso</td></tr></table></td></tr></table><!--[if (gte mso 9)|(IE)]></td></tr></table><![endif]--></td></tr><tr> <td class=\"innerpadding borderbottom\"><table width=\"115\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"> <tr><td height=\"115\" style=\"padding: 0 20px 20px 0;\"> <img src=\"http://finderjobs.com.br/images/icone-acesso.png\" width=\"115\" height=\"115\" border=\"0\" alt=\"\"/></td></tr></table><!--[if (gte mso 9)|(IE)]> <table width=\"380\" align=\"left\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr> <td><![endif]--><table class=\"col380\" align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; max-width: 380px;\"> <tr><td> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr> <td class=\"bodycopy\">#mensagemTexto </td></tr><tr> <td style=\"padding: 20px 0 0 0;\"><table class=\"buttonwrapper\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr><td> <a href=\"#link\" class=\"unsubscribe\"> <font color=\"#000\">Clique aqui</font> </a></td></tr></table> </td></tr></table></td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr></table><!--[if (gte mso 9)|(IE)]></td></tr></table><![endif]--> </body></html>";
            emailConteudo = emailConteudo.Replace("#mensagemTexto", "Por favor, cadastre uma nova senha clicando no botão abaixo");
            emailConteudo = emailConteudo.Replace("#link", callbackUrl);            
            await UserManager.SendEmailAsync(user.Id, emailTitulo, emailConteudo);

            return Json(new { sucesso = true, mensagem = "Enviado email de recuperação de senha" }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
                        
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPasswordApi(string Email, string Password, string Code)
        {            
            var user = await UserManager.FindByNameAsync(Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Json(new { sucesso = true, mensagem = "" }, JsonRequestBehavior.AllowGet);
            }
            Code = HttpUtility.UrlDecode(Code);
            var result = await UserManager.ResetPasswordAsync(user.Id, Code, Password);
            if (result.Succeeded)            
                return Json(new { sucesso = true, mensagem = "" }, JsonRequestBehavior.AllowGet);
            
            return Json(new { sucesso = true, mensagem = result.Errors.FirstOrDefault() }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInHelper.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            // Generate the token and send it
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await SignInHelper.SendTwoFactorCode(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInHelper.ExternalSignIn(loginInfo, isPersistent: false);
            switch (result)
            {
                case FinderJobs.Manager.Models.SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case FinderJobs.Manager.Models.SignInStatus.LockedOut:
                    return View("Lockout");
                case FinderJobs.Manager.Models.SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case FinderJobs.Manager.Models.SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInHelper.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}