using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage2.Data;
using Garage2.Models;

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

        // GET: Vehicles
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

            return vehicles != null ?
                          View("Index", vehicles) :
                          Problem("Entity set 'Garage2Context.Vehicle'  is null.");
        }

        // GET: Vehicles/Unparking/5
        public async Task<IActionResult> Unparking(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
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
                return Problem("Entity set 'Garage2Context.Vehicle'  is null.");
            }
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicle.Remove(vehicle);
                await _context.SaveChangesAsync();

                TempData["AlertMessage"] = "Vehicle unparking successfully";
            }

            return RedirectToAction(nameof(Index));
        }



    }
}
