using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using RDotNet;

namespace Movies.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Predict()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Predict(int budget, double vote)
        {
            if (budget == 0 || vote == 0 || budget < 100000 || vote < 2 || vote > 9) return View();
            Movie movie = new Movie
            {
                Budget = budget,
                Vote = vote
            };
            RCaller rCaller = new RCaller(@"C:/Mirvoda/revenuePredictor.R");
            movie.Revenue = rCaller.GetPrediction(movie.Budget, movie.Vote).AsInteger()[0];
            return View("Index", movie);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}