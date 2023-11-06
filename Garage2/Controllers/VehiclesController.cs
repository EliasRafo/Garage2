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
using Garage2.Models.ViewModels;
using Newtonsoft.Json;
using static Azure.Core.HttpHeader;

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
            if (_context.Vehicle != null)
            {
                return View(await _context.Vehicle.ToListAsync());
            }
            else
            {
                Feedback feedback = new Feedback() { status = "error", message = "Entity set 'Garage2Context.Vehicle'  is null." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);

                return View();
            }
        }

        // GET: Sort
        public async Task<IActionResult> Sort(string sortOrder)
        {

            if (String.IsNullOrEmpty(sortOrder)) ViewBag.TypeSortParm = "type_desc";
            else
            {
                ViewBag.TypeSortParm = sortOrder == "type" ? "type_desc" : "type";
            }

            if (String.IsNullOrEmpty(sortOrder)) ViewBag.RegNumSortParm = "regnum_desc";
            else
            {
                ViewBag.RegNumSortParm = sortOrder == "regnum" ? "regnum_desc" : "regnum";
            }

            if (String.IsNullOrEmpty(sortOrder)) ViewBag.ParkingTimeSortParm = "date_desc";
            else
            {
                ViewBag.ParkingTimeSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            }

            var vehicles = await _context.Vehicle.ToListAsync();

            switch (sortOrder)
            {
                case "type_desc":
                    vehicles = vehicles.OrderByDescending(s => s.Type).ToList();
                    break;
                case "regnum_desc":
                    vehicles = vehicles.OrderByDescending(s => s.RegNum).ToList();
                    break;
                case "date_desc":
                    vehicles = vehicles.OrderByDescending(s => s.ParkingTime).ToList();
                    break;
                case "regnum":
                    vehicles = vehicles.OrderBy(s => s.RegNum).ToList();
                    break;
                case "Date":
                    vehicles = vehicles.OrderBy(s => s.ParkingTime).ToList();
                    break;
                default:
                    vehicles = vehicles.OrderBy(s => s.Type).ToList();
                    break;
            }

            if (vehicles != null)
            {
                return View("Index", vehicles);
            }
            else
            {
                Feedback feedback = new Feedback() { status = "error", message = "Entity set 'Garage2Context.Vehicle'  is null." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);

                return View("Index");
            }
        }

        // GET: Vehicles/Unparking/5
        public async Task<IActionResult> Unparking(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                Feedback feedback = new Feedback() { status = "error", message = "Vehicle not found." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);

                return View("Index");
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                Feedback feedback = new Feedback() { status = "error", message = "Vehicle not found." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);

                return View("Index");
            }

            return View(vehicle);
        }

        // POST: Vehicles/Unparking/5
        [HttpPost, ActionName("Unparking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnparkingConfirmed(int id)
        {
            if (_context.Vehicle == null)
            {
                Feedback feedback = new Feedback() { status = "error", message = "Entity set 'Garage2Context.Vehicle'  is null." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);
                return View("Index");
            }
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicle.Remove(vehicle);
                await _context.SaveChangesAsync();

                Feedback feedback = new Feedback() { status="ok", message= "Vehicle unparkered successfully." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);

                DateTime CheckOut = DateTime.Now;

                TimeSpan duration = CheckOut - vehicle.ParkingTime;

                var pr = Math.Floor(duration.TotalMinutes * 1);

                var model = new ReceiptViewModel()
                {
                    Vehicle = vehicle,
                    CheckOutTime = CheckOut,
                    ParkingPeriod = $"{duration.Days} Days, {duration.Hours} Hours, {duration.Minutes} Minutes",
                    Price = $"{pr} SEK"
                };

                return View("Receipt", model);
            }

            return RedirectToAction(nameof(Index));
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
