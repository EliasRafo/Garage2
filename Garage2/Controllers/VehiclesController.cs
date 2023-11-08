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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Garage2.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Garage2Context _context;

        public VehiclesController(Garage2Context context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,RegNum,Color,Brand,Model,WheelsNumber")] Vehicle vehicle)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var vehicleInDb = await _context.Vehicle.AsNoTracking().SingleOrDefaultAsync(v => v.VehicleId == vehicle.VehicleId);
                if (vehicleInDb == null)
                {
                    return NotFound();
                }

                vehicle.ParkingTime = vehicleInDb.ParkingTime;

                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Vehicle.Any(e => e.VehicleId == vehicle.VehicleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
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

            //Testing
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
                .FirstOrDefaultAsync(m => m.VehicleId == id);
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
        public async Task<IActionResult> Park(Vehicle vehicle, int garageID)
        {
            vehicle.ParkingTime = DateTime.Now;
            vehicle.VehicleId = garageID;
            if (ModelState.IsValid)
            {
                if(!await VehicleExists(vehicle))
                {
                    _context.Add(vehicle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index),"Overview");
                }
                else
                {
                    Feedback feedback = new Feedback() { status = "ok", message = $"Vehicle with registration number {vehicle.RegNum} already exist in the garage." };
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);
                }
                
            }

            return View(vehicle);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string vehicleProp)
        {

            if ( vehicleProp == null || _context.Vehicle == null)
            {
                vehicleProp = string.Empty;
                Feedback feedback = new Feedback() { status = "ok", message = $"No Match found for \"{vehicleProp}\"." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);
                return View(nameof(Index));
            }

            var vehicles = await SearchMatchAsync(vehicleProp);

            if (vehicles.Count == 0)
            {
                
                Feedback feedback = new Feedback() { status = "ok", message = $"No Match found for \"{vehicleProp}\"." };
                TempData["AlertMessage"] = JsonConvert.SerializeObject(feedback);
                return View(nameof(Index));
            }

            return View(nameof(Index), vehicles.ToList());
        }

        private async Task<bool> VehicleExists(Vehicle vehicle)
        {
            return await _context.Vehicle.AnyAsync(e => e.RegNum == vehicle.RegNum);
        }

        private async Task <List<Vehicle>> SearchMatchAsync(string searchInput)
        {

            ArgumentNullException.ThrowIfNull(searchInput, nameof(searchInput));
           

            int.TryParse(searchInput,out int searchInt);


            return await _context.Vehicle
                   .Where(s => s.Brand == searchInput ||
                               s.Color == searchInput ||
                               s.Model == searchInput ||
                               s.RegNum == searchInput ||
                               s.WheelsNumber == searchInt ||
                               s.VehicleId == searchInt ||
                               (int)s.Type == searchInt).ToListAsync();
        }

    }
}
