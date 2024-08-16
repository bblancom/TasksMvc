using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TasksMvc.Models;
using TasksMvc.Services;

namespace TasksMvc.Controllers
{
	public class UsersController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly ApplicationDbContext _applicationDbContext;

		public UsersController(UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			ApplicationDbContext applicationDbContext)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_applicationDbContext = applicationDbContext;
		}

		[AllowAnonymous]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = new IdentityUser()
			{
				UserName = model.Email,
				Email = model.Email
			};
			var result = await _userManager.CreateAsync(user, password: model.Password);

			if (!result.Succeeded)
			{
				// We add all model errors
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}

				return View(model);
			}

			// If everything went well, we sign in and go to home page
			await _signInManager.SignInAsync(user, isPersistent: false);

			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		public IActionResult Login(string message = null)
		{
			if (message is not null)
			{
				ViewData["Message"] = message;
			}

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await _signInManager.PasswordSignInAsync(model.Email,
				model.Password, model.RememberMe, lockoutOnFailure: false);

			if (!result.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "Username or password incorrect.");

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		[HttpGet]
		public ChallengeResult LoginExternal(string provider, string returnUrl = null)
		{
			var urlRedirection = Url.Action("RegisterExternalUser", values: new { returnUrl });
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, urlRedirection);

			return new ChallengeResult(provider, properties);
		}


		[AllowAnonymous]
		public async Task<IActionResult> RegisterExternalUser(string returnUrl = null, string remoteError = null)
		{
			var defaultReturnUrl = returnUrl ?? Url.Content("~/");
			var errorMessage = string.Empty;

			if (remoteError is not null)
			{
				errorMessage = $"Error from the external provider: [{remoteError}]";
				return RedirectToAction("login", routeValues: new { errorMessage });
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info is null)
			{
				errorMessage = "Error loading data from external login";
				return RedirectToAction("login", routeValues: new { errorMessage });
			}

			var resultExternalLogin = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
				info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

			// Account already exists
			if (resultExternalLogin.Succeeded)
			{
				return LocalRedirect(defaultReturnUrl);
			}

			if (!info.Principal.HasClaim(claim => claim.Type == ClaimTypes.Email))
			{
				errorMessage = "Error reading email from provider";
				return RedirectToAction("login", routeValues: new { errorMessage });
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			var user = new IdentityUser { Email = email, UserName = email };
			var resultCreateUser = await _userManager.CreateAsync(user);

			if (!resultCreateUser.Succeeded)
			{
				errorMessage = resultCreateUser.Errors.FirstOrDefault()?.Description;
				return RedirectToAction("login", routeValues: new { errorMessage });
			}

			var resultAddLogin = await _userManager.AddLoginAsync(user, info);

			if (!resultAddLogin.Succeeded)
			{
				errorMessage = "Error adding the login to the user";
				return RedirectToAction("login", routeValues: new { errorMessage });
			}

			await _signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);

			return LocalRedirect(defaultReturnUrl);
		}

		[HttpGet]
		[Authorize(Roles = Constants.RoleAdmin)]
		public async Task<IActionResult> List(string message = null)
		{
			var users = await _applicationDbContext.Users.Select(user => new UserViewModel
			{
				Email = user.Email,
			}).ToListAsync();

			var model = new UsersListViewModel
			{
				Users = users,
				Message = message
			};

			return View(model);
		}

		[HttpPost]
		[Authorize(Roles = Constants.RoleAdmin)]
		public async Task<IActionResult> MakeAdmin(string email)
		{
			var user = await _applicationDbContext.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

			if (user is null)
			{
				return NotFound();
			}

			await _userManager.AddToRoleAsync(user, Constants.RoleAdmin);

			return RedirectToAction("List",
				routeValues: new { message = "Role assigned successfully to " + email });
		}

		[HttpPost]
		[Authorize(Roles = Constants.RoleAdmin)]
		public async Task<IActionResult> RemoveAdmin(string email)
		{
			var user = await _applicationDbContext.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

			if (user is null)
			{
				return NotFound();
			}

			await _userManager.RemoveFromRoleAsync(user, Constants.RoleAdmin);

			return RedirectToAction("List",
				routeValues: new { message = "Role removed successfully to " + email });
		}
	}
}