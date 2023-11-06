using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage2.Data;
using Garage2.Models;
using Humanizer;
using System.ComponentModel;

namespace Garage2.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Garage2Context _context;

        public VehiclesController(Garage2Context context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
              return _context.Vehicle != null ? 
                          View(await _context.Vehicle.ToListAsync()) :
                          Problem("Entity set 'Garage2Context.Vehicle'  is null.");
        }

        [HttpGet]
        public IActionResult Park()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Park(Vehicle vehicle)
        {
            vehicle.ParkingTime = DateTime.Now;

            if (ModelState.IsValid)
            {
                if(!VehicleExists(vehicle))
                {
                    _context.Add(vehicle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //Replace with alert?
                    return NotFound();
                }
                
            }

            return View(vehicle);
        }

        [HttpGet]
        //Under development
        //been moved in to methods:
        //Create Model Properties for search
        public async Task<IActionResult> Search(string vehicleProp)
        {
            //if (vehicleProp == null || _context.Vehicle == null)
            //{
            //    return NotFound();
            //}
            // Under Development:
            var ContextVehicles = await _context.Vehicle.ToListAsync();
            IEnumerable<Vehicle> vehicles = ContextVehicles
                .Where(p => p.ToString() == vehicleProp);
            

            return View(vehicles);
        }

        private bool VehicleExists(Vehicle vehicle)
        {
            return (_context.Vehicle?.Any(e => e.RegNum == vehicle.RegNum)).GetValueOrDefault();
        }

        private List<Vehicle>Searchmatch()

    }
}
