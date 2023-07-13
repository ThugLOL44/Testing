﻿using Application.Interfaces;
using Application.Interfaces.IApi;
using Application.Request;
using Application.Response;
using Domain.Entities;
using Newtonsoft.Json.Linq;

namespace Application.UseCase.Viajes
{
    public class ViajeService : IViajeService
    {
        private readonly IViajeCommand _command;
        private readonly IViajeQuery _query;

        private readonly IDestinoApi _destinoApi;
        private readonly IServicioApi _servicioApi;
        private readonly ITransporteApi _transporteApi;

        public ViajeService(IViajeCommand command, IViajeQuery query, IDestinoApi destinoApi, IServicioApi servicioApi, ITransporteApi transporteApi)
        {
            _command = command;
            _query = query;
            _destinoApi = destinoApi;
            _servicioApi = servicioApi;
            _transporteApi = transporteApi;
        }

        public ViajeCompletoResponse GetViajeById(int viajeId)
        {
            var viaje = _query.GetViajeById(viajeId);

            if (viaje == null)
            {
                throw new ArgumentException($"No se encontró el viaje con el identificador {viajeId}.");
            }

            return MappingViaje(viaje);
        }

        public List<ViajeCompletoResponse> GetViajeListFilters(string tipo, string fechaSalida, string fechaLlegada, string empresa, string compania, int ciudadOrigen, int ciudadDestino, int pasajesDisponibles, string orden)
        {
            var viajeList = _query.GetViajeListFilters(tipo, fechaSalida, fechaLlegada, empresa, compania, ciudadOrigen, ciudadDestino, pasajesDisponibles, orden);

            List<ViajeCompletoResponse> viajeResponseList = new List<ViajeCompletoResponse>();

            foreach (var viaje in viajeList)
            {
                viajeResponseList.Add(MappingViaje(viaje));
            }

            return viajeResponseList;
        }

        public ViajeResponse CreateViaje(ViajeRequest request)
        {
            var listaJsonTransporte = _transporteApi.ObtenerCaracteristicaTransporteList();

            string valorResponse = "";

            foreach (var json in listaJsonTransporte)
            {
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                JToken token = JToken.Parse(jsonString);

                int caracteristicaId = (int)token.SelectToken("caracteristicaId");
                int transporteId = (int)token.SelectToken("transporteId");
                string valor = (string)token.SelectToken("valor");

                if (caracteristicaId == 1 && transporteId == request.TransporteId)
                {
                    valorResponse = valor;
                }
            }

            var viaje = new Viaje
            {
                TransporteId = request.TransporteId,
                Duracion = request.Duracion,
                FechaLlegada = DateTime.Parse(request.FechaLlegada),
                FechaSalida = DateTime.Parse(request.FechaLlegada),
                TipoViaje = request.TipoViaje,
                AsientosDisponibles = int.Parse(valorResponse),
                Precio = request.Precio,
            };

            var viajeInserte = _command.InsertViaje(viaje);

            _destinoApi.CreateViajeCiudad(viajeInserte.ViajeId, request.CiudadOrigen, "Origen");

            foreach (var escala in request.Escalas)
            {
                _destinoApi.CreateViajeCiudad(viajeInserte.ViajeId, escala, "Escala");
            }

            _destinoApi.CreateViajeCiudad(viajeInserte.ViajeId, request.CiudadDestino, "Destino");

            foreach (var servicio in request.Servicios)
            {
                _servicioApi.CreateViajeServicio(viajeInserte.ViajeId, servicio);
            }

            return new ViajeResponse
            {
                Id = viaje.ViajeId,
                TransporteId = viaje.TransporteId,
                Duracion = viaje.Duracion,
                FechaSalida = viaje.FechaSalida,
                FechaLlegada = viaje.FechaLlegada,
                TipoViaje = viaje.TipoViaje,
                AsientosDisponibles = viaje.AsientosDisponibles,
                Precio = viaje.Precio,
                CiudadOrigen = request.CiudadOrigen,
                CiudadDestino = request.CiudadDestino,
                Escalas = request.Escalas,
                Servicios = request.Servicios,
            };
        }

        public ViajeCompletoResponse RemoveViaje(int viajeId)
        {
            if (_query.GetViajeById(viajeId) == null)
            {
                throw new ArgumentException($"No se encontró el viaje que desea eliminar con el identificador '{viajeId}'.");
            }

            var viaje = _command.RemoveViaje(viajeId);

            return MappingViaje(viaje);
        }

        public ViajeCompletoResponse UpdateViaje(int viajeId, int asientosDisponibles)
        {
            var viaje = _query.GetViajeById(viajeId);

            if (viaje == null)
            {
                throw new ArgumentException($"No se encontró el viaje con el identificador {viajeId}.");
            }

            int asientos = viaje.AsientosDisponibles -= asientosDisponibles;

            viaje.AsientosDisponibles = asientos;

            _command.UpdateViaje(viaje);

            return MappingViaje(viaje);
        }

        private ViajeCompletoResponse MappingViaje(Viaje viaje)
        {
            var listaJsonViajes = _destinoApi.ObtenerViajeList(viaje.ViajeId);

            var listaJsonServicio = _servicioApi.ObtenerServicioList(viaje.ViajeId);

            int ciudadOrigenResponse = 0;
            int ciudadDestinoResponse = 0;

            List<int> escalas = new List<int>();
            List<int> servicios = new List<int>();

            foreach (object json in listaJsonViajes)
            {
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                JToken token = JToken.Parse(jsonString);

                int idCiudad = (int)token.SelectToken("ciudad.id");
                string tipoCiudad = (string)token.SelectToken("tipo");

                if (tipoCiudad == "Origen")
                {
                    ciudadOrigenResponse = idCiudad;
                }

                if (tipoCiudad == "Destino")
                {
                    ciudadDestinoResponse = idCiudad;
                }

                if (tipoCiudad == "Escala")
                {
                    escalas.Add(idCiudad);
                }
            }

            foreach (var json in listaJsonServicio)
            {
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                JToken token = JToken.Parse(jsonString);

                int idServicio = (int)token.SelectToken("servicioId");

                servicios.Add(idServicio);
            }

            var JsonTransporte = _transporteApi.ObtenerTransporte(viaje.TransporteId);
            string jsonTipoTransporte = Newtonsoft.Json.JsonConvert.SerializeObject(JsonTransporte);
            JToken tokenTransporte = JToken.Parse(jsonTipoTransporte);
            string DescripcionTransporte = (string)tokenTransporte.SelectToken("tipoTransporteResponse.descripcion");


            var jsonCiudad = _destinoApi.ObtenerCiudadDestino(ciudadDestinoResponse);
            string jsonDescripcionCiudad = Newtonsoft.Json.JsonConvert.SerializeObject(jsonCiudad);
            JToken tokenCiudad = JToken.Parse(jsonDescripcionCiudad);
            string descripcionCiudad = (string)tokenCiudad.SelectToken("nombre");
            string imagenCiudad = "";
       

                var jsonInfoCiudad = _destinoApi.ObtenerInfoCiudadDestino(ciudadDestinoResponse);
                string jsonImagenCiudad = Newtonsoft.Json.JsonConvert.SerializeObject(jsonInfoCiudad);
                JToken tokenInfoCiudad = JToken.Parse(jsonImagenCiudad);
                imagenCiudad = (string)tokenInfoCiudad.SelectToken("imagen");
            




            return new ViajeCompletoResponse
            {
                Id = viaje.ViajeId,
                TransporteId = viaje.TransporteId,
                TipoTransporte = DescripcionTransporte,
                Duracion = viaje.Duracion,
                FechaSalida = viaje.FechaSalida,
                FechaLlegada = viaje.FechaLlegada,
                TipoViaje = viaje.TipoViaje,
                AsientosDisponibles = viaje.AsientosDisponibles,
                Precio = viaje.Precio,
                CiudadOrigen = ciudadOrigenResponse,
                CiudadDestino = ciudadDestinoResponse,
                CiudadDestinoDescripcion= descripcionCiudad,
                CiudadDestinoImagen = imagenCiudad,
                Escalas = escalas,
                Servicios = servicios,
            };
        }
    }
}
