﻿using Garage2.Data;
using Garage2.Models;
using Garage2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Configuration;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Runtime.Intrinsics.X86;
using System.Reflection.Emit;

namespace Garage2.Controllers
{
    public class OverviewController : Controller
    {
        private readonly Garage2Context _context;
        private readonly IConfiguration _iConfig;

        public OverviewController(Garage2Context context, IConfiguration iConfig)
        {
            _context = context;
            _iConfig = iConfig;
        }

        public async Task<IActionResult> Index()
        {
            OverviewViewModel overviewViewModel = new OverviewViewModel();

            if (int.TryParse(_iConfig.GetSection("Garage").GetSection("Capacity").Value, out int result))
                overviewViewModel.Capacity = result;
            else
                overviewViewModel.Capacity = 0;

            overviewViewModel.VehiclesStatistic = await GetVehicleTypes();

            overviewViewModel.WheelsNumber = await GetWheelsNumber();

            overviewViewModel.Revenues = await GetRevenues();

            int count = await _context.Vehicle.CountAsync();
            overviewViewModel.FreeSpaces = overviewViewModel.Capacity - count;

            overviewViewModel.ParkingSpaces = await GetParkingSpaces();

            return View(nameof(Index), overviewViewModel);
        }

        public async Task<List<VehicleTypesDto>> GetVehicleTypes()  
        {
            return await _context.Vehicle.GroupBy(x => x.Type).Select(grp => new VehicleTypesDto(grp.Key, grp.Count())).ToListAsync();
        }

        public async Task<int> GetWheelsNumber()
        {
            return await _context.Vehicle.SumAsync(v => v.WheelsNumber);
        }

        public async Task<double> GetRevenues()
        {
            var vehicles = await _context.Vehicle.ToListAsync();
            DateTime CheckOut = DateTime.Now;
            double revenues = 0;
            foreach (Vehicle v in  vehicles)
            {
                TimeSpan duration = CheckOut - v.ParkingTime;
                revenues += Math.Floor(duration.TotalMinutes * 1);
            }

            return revenues;
        }

        public async Task<List<ParkingSpace>> GetParkingSpaces()
        {
            List<ParkingSpace> parkingSpaces = new List<ParkingSpace>();

            var vehicles = await _context.Vehicle.ToListAsync();
            int Capacity;

            if (int.TryParse(_iConfig.GetSection("Garage").GetSection("Capacity").Value, out int result)) 
                Capacity = result;
            else
                Capacity = 0;

            for (int i = 1; i <= Capacity* SizeData.Full; i++) 
            {
                var vehic = vehicles.Where(e => e.Address == i); //Comment Development of size of parking spots.FirstOrDefault();
                ParkingSpace parkingSpace = new ParkingSpace();
                //Think about foreach, move size of lot/asigning of lot to here?
                foreach(var v in vehic)
                { 
                    if (v is null)
                    {
                        parkingSpace.Id = i;
                        parkingSpace.Reserved = false;
                        parkingSpace.Vehicle = null;
                        parkingSpace.SpaceOccupied = SizeData.Empty; 
                    }
                    else
                    {
                        parkingSpace.Id = i;
                        parkingSpace.Vehicle = v;
                        //Should be moved
                        int parkingSize = 0;
                        if (parkingSpace.SpaceOccupied != SizeData.Full)
                        {
                            parkingSize = SizeData.AssignSize(v.Type);
                            parkingSpace.SpaceOccupied += parkingSize;
                        }
                        if (parkingSpace.SpaceOccupied == SizeData.Full)
                        {
                            parkingSpace.Reserved = true;
                        }
                        //Should be moved.
                    }
                    parkingSpaces.Add(parkingSpace);
                }
            }

            return parkingSpaces;
        }
    }
}
