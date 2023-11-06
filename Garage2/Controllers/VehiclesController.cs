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
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

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
        // Investigate use of async for this action.
        public async Task<IActionResult> Search(string vehicleProp)
        {
            var vehicles =  Searchmatch(vehicleProp);
            //if (vehicleProp == null || _context.Vehicle == null || vehicles == null)
            //{
            //    return NotFound();
            //}

            return View(vehicles.ToList());
        }

        private bool VehicleExists(Vehicle vehicle)
        {
            return (_context.Vehicle?.Any(e => e.RegNum == vehicle.RegNum)).GetValueOrDefault();
        }

        // Change to async
        private List<Vehicle> Searchmatch(string searchInput)
        {
            var contextVehicles =  _context.Vehicle.ToList();

            int searchInpuInt;
            Types searchType;

            List<Vehicle>? vehicles = null;
            List < Vehicle >? vehiclesInt = null;
            List<Vehicle>? vehiclesString = null; //Brand, Color, RegNum Model
            List<Vehicle>? vehiclesType = null;
            //Add search, time of parking event.
            //List<Vehicle>? vehiclesDateTime = null;

            bool isInt = int.TryParse(searchInput, out searchInpuInt);
            
            if(isInt)
            {

                vehiclesInt = contextVehicles
                   .Where(s => s.WheelsNumber == searchInpuInt ||
                               s.Id == searchInpuInt).ToList();
            }

            vehiclesString = contextVehicles
                   .Where(s => s.Brand == searchInput ||
                               s.Color == searchInput ||
                               s.Model == searchInput ||
                               s.RegNum == searchInput).ToList();

            if (Enum.TryParse<Types>(searchInput, true, out searchType))  // ignore cases
            {
                vehiclesType = contextVehicles
                       .Where(s => s.Type == searchType).ToList();
            }

            if (vehiclesInt != null)
            {
                vehicles = vehiclesInt;
            }

            if (vehiclesString != null) 
            {
                if(vehicles!= null)
                {
                    vehicles.AddRange(vehiclesString);
                }
                else
                {
                    vehicles = vehiclesString;
                }
            }

            if (vehiclesType != null)
            {
                if (vehicles != null)
                {
                    vehicles.AddRange(vehiclesType);
                }
                else
                {
                    vehicles = vehiclesType;
                }
            }


            //int
            //datetime
            //Types
            //String

            return vehicles;
        }

    }
}
