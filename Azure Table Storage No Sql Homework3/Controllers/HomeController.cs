using Azure_Table_Storage_No_Sql_Homework3.Models;
using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Azure_Table_Storage_No_Sql_Homework3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INoSqlStorage<Product> _product;
        private readonly INoSqlStorage<Store> _store;

        public HomeController(ILogger<HomeController> logger, INoSqlStorage<Product> product, INoSqlStorage<Store> store)
        {
            _logger = logger;
            _product = product;
            _store = store;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddStore() 
        {
            var vm = new AddStoreViewModel
            {
                Store = new Store()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddStore(AddStoreViewModel vm) 
        {
            if (ModelState.IsValid)
            {
                await _store.Add(vm.Store);
                return RedirectToAction("Index", "Home");
            }
            
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Stores = await _store.All();
            var vm = new AddProductViewModel
            {
                Product = new Product()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _product.Add(vm.Product);
                return RedirectToAction("Index", "Home");
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
