﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Admin.Aan.Web.Infrastructure;
using SFA.DAS.Admin.Aan.Web.Models;

namespace SFA.DAS.Admin.Aan.Web.Controllers;

[Authorize]
[Route("admin-hub", Name = RouteNames.AdministratorHub)]
public class AdminHubController : Controller
{
    public IActionResult Index()
    {
        return View(new AdminHubViewModel());
    }
}
