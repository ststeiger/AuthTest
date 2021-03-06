﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthTest.Models;


namespace AuthTest.Controllers
{


    public class HomeController : Controller
    {
        
        
        public IActionResult Index()
        {
            try
            {
                // string usr = this.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                string usr = this.User.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                
                System.Console.WriteLine(usr);
            }
            catch (Exception e)
            {
                // System.Console.WriteLine(e);
                throw;
            }
            
            return View();
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }


        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    } // End Class HomeController : Controller 


} // End Namespace AuthTest.Controllers 
