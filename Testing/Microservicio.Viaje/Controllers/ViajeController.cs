﻿using Application.Interfaces;
using Application.Request;
using Application.Response;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio_Viaje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViajeController : ControllerBase
    {
        private readonly IViajeService _service;

        public ViajeController(IViajeService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ViajeCompletoResponse), 200)]
        public IActionResult GetViajeListFilters(string? tipo, string? fechaSalida, string? fechaLlegada, string? empresa,string? compania ,int ciudadOrigen, int ciudadDestino, int pasajesDisponibles,string? orden = "Menor Precio")
        {
            try
            {
                var result = _service.GetViajeListFilters(tipo, fechaSalida, fechaLlegada, empresa, compania, ciudadOrigen, ciudadDestino, pasajesDisponibles, orden);
                return new JsonResult(result);
            }

            catch (Exception ex)
            {
                return NotFound(new BadRequest
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ViajeResponse), 201)]
        public IActionResult CreateViaje(ViajeRequest request)
        {
            var result = _service.CreateViaje(request);
            return new JsonResult(result) { StatusCode = 201 };

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ViajeCompletoResponse), 200)]
        [ProducesResponseType(typeof(BadRequest), 404)]
        public IActionResult GetViajeById(int id)
        {
            try
            {
                var result = _service.GetViajeById(id);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return NotFound(new BadRequest
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ViajeCompletoResponse), 200)]
        [ProducesResponseType(typeof(BadRequest), 404)]
        public IActionResult UpdateViaje(int id, int asientosDisponibles)
        {
            try
            {
                var result = _service.UpdateViaje(id, asientosDisponibles);
                return new JsonResult(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new BadRequest
                {
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ViajeCompletoResponse), 200)]
        [ProducesResponseType(typeof(BadRequest), 404)]
        public IActionResult RemoveViaje(int id)
        {
            try
            {
                var result = _service.RemoveViaje(id);
                return new JsonResult(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new BadRequest
                {
                    Message = ex.Message
                });
            }
        }
    }
}
