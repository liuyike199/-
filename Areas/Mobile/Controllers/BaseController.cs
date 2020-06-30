using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreFrame.Areas.Mobile.Controllers
{
    [Area("Mobile")]
    [Route("Mobile/[controller]/[action]")]
    public class BaseController : Controller
    {
    }
}
