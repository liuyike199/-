using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace CoreFrame.Areas.Mobile.Controllers
{
    public class HomeController : BaseController
    {
        public IMenuService _menuService { get; set; }
        public HomeController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var model = _menuService.GetIndex();
            return View(model);
        }
    }
}
