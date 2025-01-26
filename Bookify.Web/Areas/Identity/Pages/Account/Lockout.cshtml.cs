// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookify.Web.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI InfrastructureLayer and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous]
    public class LockoutModel : PageModel
    {
        public TimeOnly LockedEndTime { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI InfrastructureLayer and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IActionResult OnGet(DateTimeOffset lockedEndTime)
        {
            var userLockedOutTime = lockedEndTime - DateTime.Now;
            if (userLockedOutTime.Ticks < 0)
                return RedirectToPage("./login");


            TimeOnly lockedOutEndTime = TimeOnly.FromTimeSpan(userLockedOutTime);

            LockedEndTime = lockedOutEndTime;

            return Page();



        }
    }
}
