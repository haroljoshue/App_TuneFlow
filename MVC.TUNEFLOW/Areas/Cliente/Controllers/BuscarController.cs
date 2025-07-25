﻿using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using System.Diagnostics;
using System.Security.Claims;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Production;



namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class BuscarController : Controller
    {
            public async Task<IActionResult> Index()
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
               
            if (string.IsNullOrEmpty(userId))
            {
                
                return RedirectToAction("Login", "Account");
                }
            var client = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
         
          
            if (client == null)
            {
                Console.WriteLine("p4");
                return RedirectToAction("Index", "Buscar");
                }
            
            ViewBag.IdCliente = client.Id;
            Console.WriteLine($"ViewBag: {ViewBag.IdClient}");
            
            return View(new List<Song>());
            }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Buscar(string nameSong)
        {
            Console.WriteLine($"Buscar llamado con parámetro: '{nameSong}'");
            if (string.IsNullOrWhiteSpace(nameSong))
            {
                Console.WriteLine("Error: parámetro vacío");
                return View("Index", new List<Song>());
            }
          
            var songs = await Crud<Song>.GetCancionesPorgenero(nameSong);
            foreach(var song in songs)
            {
                song.Artist = await Crud<Artist>.GetByIdAsync(song.ArtistId);
            }
            Console.WriteLine($"Buscar llamado con parámetro: '{nameSong}'");
            Console.WriteLine($"Número de canciones recibidas en controlador: {songs?.Count ?? 0}");
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var client = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);

            if (client == null)
            {
                return RedirectToAction("Index", "Buscar");
            }

            ViewBag.IdCliente = client.Id;
            Console.WriteLine($"ViewBag: {ViewBag.IdClient}");

            return View("Index", songs);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(string nameSong)
        {
            Console.WriteLine($"Buscar llamado con parámetro: '{nameSong}'");
            if (string.IsNullOrWhiteSpace(nameSong))
            {
                Console.WriteLine("Error: parámetro vacío");
                return View("Index", new List<Song>());
            }

            var songs = await Crud<Song>.GetCancionesPorPalabrasClave(nameSong);
            Console.WriteLine($"Buscar llamado con parámetro: '{nameSong}'");
            Console.WriteLine($"Número de canciones recibidas en controlador: {songs?.Count ?? 0}");
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var client = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);

            if (client == null)
            {
                return RedirectToAction("Index", "Buscar");
            }

            ViewBag.IdCliente = client.Id;
            Console.WriteLine($"ViewBag: {ViewBag.IdCliente}");

            return View("Index", songs);
        }
    }

}
