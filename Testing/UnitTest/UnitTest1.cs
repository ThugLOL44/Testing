using Application.Interfaces;
using Application.Interfaces.IApi;
using Application.Request;
using Application.UseCase.Pasajeros;
using Application.UseCase.Viajes;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void TestGetPasajeroById_Null()
        {
            //ARRANGE
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();
            //ACT
            var pasajeroId = 1;

            mockPasajeroQuery.Setup(q => q.GetPasajeroById(pasajeroId)).Returns<Pasajero>(null);

            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ASSERT
            var exception = Assert.Throws<ArgumentException>(() => pasajeroService.GetPasajeroById(pasajeroId));
            exception.Message.Should().Be($"No se encontró el pasajero con el identificador {pasajeroId}.");
        }

        [Fact]
        public void TestGetPasajeroById()
        {
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var pasajeroId = 1;
            var pasajero = new Pasajero
            {
                PasajeroId = pasajeroId,
                Nombre = "John",
                Apellido = "Doe",
                Dni = 123456789,
                FechaNacimiento = new DateTime(1990, 10, 15),
                Genero = "Masculino",
                NumContactoEmergencia = 987654321,
                Nacionalidad = "Argentina",
                ViajeId = 1,
                Viaje = new Viaje
                {
                    ViajeId = 1,
                    TransporteId = 2,
                    Duracion = "2 horas",
                    FechaLlegada = DateTime.Now.AddDays(1),
                    FechaSalida = DateTime.Now,
                    TipoViaje = "Aéreo",
                    AsientosDisponibles = 100,
                    Precio = 200,
                    Pasajeros = new List<Pasajero>()
                }

            };

            mockPasajeroQuery.Setup(q => q.GetPasajeroById(pasajeroId)).Returns(pasajero);

            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ACT
            var result = pasajeroService.GetPasajeroById(pasajeroId);

            //ASSERT

            // Verificar que el resultado no sea nulo
            Assert.NotNull(result);


            // Verificar las propiedades del objeto PasajeroResponse
            Assert.Equal(pasajero.PasajeroId, result.Id);
            Assert.Equal(pasajero.Nombre, result.Nombre);
            Assert.Equal(pasajero.Apellido, result.Apellido);
            Assert.Equal(pasajero.Dni, result.Dni);
            Assert.Equal(pasajero.FechaNacimiento, result.FechaNacimiento);
            Assert.Equal(pasajero.Genero, result.Genero);
            Assert.Equal(pasajero.NumContactoEmergencia, result.NumContactoEmergencia);
            Assert.Equal(pasajero.Nacionalidad, result.Nacionalidad);

            // Verificar las propiedades del objeto ViajeGetResponse dentro de PasajeroResponse
            Assert.NotNull(result.Viaje);
            Assert.Equal(pasajero.Viaje.ViajeId, result.Viaje.Id);
            Assert.Equal(pasajero.Viaje.TransporteId, result.Viaje.TransporteId);
            Assert.Equal(pasajero.Viaje.Duracion, result.Viaje.Duracion);
            Assert.Equal(pasajero.Viaje.FechaSalida, result.Viaje.FechaSalida);
            Assert.Equal(pasajero.Viaje.FechaLlegada, result.Viaje.FechaLlegada);
            Assert.Equal(pasajero.Viaje.TipoViaje, result.Viaje.TipoViaje);
            Assert.Equal(pasajero.Viaje.Precio, result.Viaje.Precio);

        }

        [Fact]
        public void TestPasajeroRemove_null()
        {
            //ARRANGE
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var pasajeroId = 1;

            mockPasajeroQuery.Setup(q => q.GetPasajeroById(pasajeroId)).Returns<Pasajero>(null);

            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ACT
            var exception = Assert.Throws<ArgumentException>(() => pasajeroService.RemovePasajero(pasajeroId));

            //ASSERT
            exception.Message.Should().Be($"No se encontro el pasajero que desea eliminar con el identificador '{pasajeroId}'.");
        }

        [Fact]
        public void TestPasajeroRemove()
        {
            //ARRANGE
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var pasajeroId = 1;
            var pasajero = new Pasajero
            {
                PasajeroId = pasajeroId,
                Nombre = "John",
                Apellido = "Doe",
                Dni = 123456789,
                FechaNacimiento = new DateTime(1990, 10, 15),
                Genero = "Masculino",
                NumContactoEmergencia = 987654321,
                Nacionalidad = "Argentina",
                ViajeId = 1,
                Viaje = new Viaje
                {
                    ViajeId = 1,
                    TransporteId = 2,
                    Duracion = "2 horas",
                    FechaLlegada = DateTime.Now.AddDays(1),
                    FechaSalida = DateTime.Now,
                    TipoViaje = "Aéreo",
                    AsientosDisponibles = 100,
                    Precio = 200,
                    Pasajeros = new List<Pasajero>()
                }

            };

            mockPasajeroQuery.Setup(q => q.GetPasajeroById(pasajeroId)).Returns(pasajero);
            mockPasajeroCommand.Setup(c => c.RemovePasajero(pasajeroId)).Returns(pasajero);

            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ACT

            var result = pasajeroService.RemovePasajero(pasajeroId);

            //ASSERT
            Assert.NotNull(result);

            Assert.Equal(pasajero.PasajeroId, result.Id);
            Assert.Equal(pasajero.Nombre, result.Nombre);
            Assert.Equal(pasajero.Apellido, result.Apellido);
            Assert.Equal(pasajero.Dni, result.Dni);
            Assert.Equal(pasajero.FechaNacimiento, result.FechaNacimiento);
            Assert.Equal(pasajero.Genero, result.Genero);
            Assert.Equal(pasajero.NumContactoEmergencia, result.NumContactoEmergencia);
            Assert.Equal(pasajero.Nacionalidad, result.Nacionalidad);

            // Verificar las propiedades del objeto ViajeGetResponse dentro de PasajeroResponse
            Assert.NotNull(result.Viaje);

            Assert.Equal(pasajero.Viaje.ViajeId, result.Viaje.Id);
            Assert.Equal(pasajero.Viaje.TransporteId, result.Viaje.TransporteId);
            Assert.Equal(pasajero.Viaje.Duracion, result.Viaje.Duracion);
            Assert.Equal(pasajero.Viaje.FechaSalida, result.Viaje.FechaSalida);
            Assert.Equal(pasajero.Viaje.FechaLlegada, result.Viaje.FechaLlegada);
            Assert.Equal(pasajero.Viaje.TipoViaje, result.Viaje.TipoViaje);
            Assert.Equal(pasajero.Viaje.Precio, result.Viaje.Precio);

            mockPasajeroCommand.Verify(c => c.RemovePasajero(pasajeroId), Times.Once);

        }


        [Fact]
        public void TestPasajeroCreate()
        {
            // ARRANGE
            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var viajeId = 1;
            var request = new PasajeroRequest
            {
                Nombre = "John",
                Apellido = "Doe",
                Dni = 123456789,
                FechaNacimiento = new DateTime(1990, 10, 15),
                Genero = "Masculino",
                NumContactoEmergencia = 987654321,
                Nacionalidad = "Argentina",
                ViajeId = viajeId,

            };

            // Configurar el mock de IViajeQuery para devolver un viaje existente
            var viajeExistente = new Viaje();
            mockViajeQuery.Setup(q => q.GetViajeById(viajeId)).Returns(viajeExistente);

            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            // ACT
            var result = pasajeroService.CreatePasajero(request);

            // ASSERT
            // Verificar que el método InsertPasajero del mock IPasajeroCommand haya sido llamado con el objeto Pasajero correcto
            mockPasajeroCommand.Verify(c => c.InsertPasajero(It.Is<Pasajero>(p =>
                p.Nombre == request.Nombre &&
                p.Apellido == request.Apellido &&
                p.Dni == request.Dni &&
                p.FechaNacimiento == request.FechaNacimiento &&
                p.Genero == request.Genero &&
                p.NumContactoEmergencia == request.NumContactoEmergencia &&
                p.Nacionalidad == request.Nacionalidad &&
                p.ViajeId == request.ViajeId &&
                p.Viaje == viajeExistente
            )));

            // Verificar que el método GetViajeById del mock IViajeQuery haya sido llamado con el ID de viaje correcto
            mockViajeQuery.Verify(q => q.GetViajeById(viajeId));

            // Verificar que se haya devuelto un objeto PasajeroResponse con los datos esperados
            Assert.NotNull(result);
            Assert.Equal(request.Nombre, result.Nombre);
            Assert.Equal(request.Apellido, result.Apellido);
            Assert.Equal(request.Dni, result.Dni);
            Assert.Equal(request.FechaNacimiento, result.FechaNacimiento);
            Assert.Equal(request.Genero, result.Genero);
            Assert.Equal(request.NumContactoEmergencia, result.NumContactoEmergencia);
            Assert.Equal(request.Nacionalidad, result.Nacionalidad);
            Assert.NotNull(result.Viaje);
            Assert.Equal(viajeExistente.ViajeId, result.Viaje.Id);
            // Verificar otras propiedades del objeto PasajeroResponse si es necesario
        }

        [Fact]
        public void TestPasajeroCreate_NullViaje()
        {
            // ARRANGE
            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var viajeId = 1;
            var request = new PasajeroRequest
            {
                Nombre = "John",
                Apellido = "Doe",
                Dni = 123456789,
                FechaNacimiento = new DateTime(1990, 10, 15),
                Genero = "Masculino",
                NumContactoEmergencia = 987654321,
                Nacionalidad = "Argentina",
                ViajeId = viajeId,

            };

            // Configurar el mock de IViajeQuery para devolver un viaje existente
            mockViajeQuery.Setup(q => q.GetViajeById(viajeId)).Returns((Viaje)null);

            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ACT
            var exception = Assert.Throws<ArgumentException>(() => pasajeroService.CreatePasajero(request));

            //ASSERT
            exception.Message.Should().Be($"No se encontró el viaje con el identificador {request.ViajeId}.");

        }

        [Fact]
        public void TestMappingPasajero()
        {
            //ARRANGE

            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var pasajeroList = new List<Pasajero>
            {
                new Pasajero
                {
                    PasajeroId = 1,
                    Nombre = "John",
                    Apellido = "Doe",
                    Dni = 123456789,
                    FechaNacimiento = new DateTime(1990, 10, 15),
                    Genero = "Masculino",
                    NumContactoEmergencia = 987654321,
                    Nacionalidad = "Argentina",
                    ViajeId = 1,
                    Viaje = new Viaje
                    {
                        ViajeId = 1,
                        TransporteId = 2,
                        Duracion = "2 horas",
                        FechaLlegada = DateTime.Now.AddDays(1),
                        FechaSalida = DateTime.Now,
                        TipoViaje = "Aéreo",
                        AsientosDisponibles = 100,
                        Precio = 200,
                        Pasajeros = new List<Pasajero>()
                    }
                },
                new Pasajero
                {
                    PasajeroId = 2,
                    Nombre = "Jane",
                    Apellido = "Smith",
                    Dni = 987654321,
                    FechaNacimiento = new DateTime(1995, 5, 20),
                    Genero = "Femenino",
                    NumContactoEmergencia = 123456789,
                    Nacionalidad = "Estados Unidos",
                    ViajeId = 2,
                    Viaje = new Viaje
                    {
                        ViajeId = 2,
                        TransporteId = 3,
                        Duracion = "3 horas",
                        FechaLlegada = DateTime.Now.AddDays(2),
                        FechaSalida = DateTime.Now,
                        TipoViaje = "Terrestre",
                        AsientosDisponibles = 50,
                        Precio = 150,
                        Pasajeros = new List<Pasajero>()
                    }
                }
            };

            mockPasajeroQuery.Setup(q => q.GetPasajeroList()).Returns(pasajeroList);

            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ACT

            var result = pasajeroService.GetPasajeroList();

            //ASSERT
            mockPasajeroQuery.Verify(q => q.GetPasajeroList());

            Assert.NotNull(result);
            Assert.Equal(pasajeroList.Count, result.Count);

            for (int i = 0; i < pasajeroList.Count; i++)
            {
                var expectedResponse = pasajeroService.MappingPasajero(pasajeroList[i]);
                var actualResponse = result[i];

                Assert.Equal(expectedResponse.Id, actualResponse.Id);
                Assert.Equal(expectedResponse.Nombre, actualResponse.Nombre);
                Assert.Equal(expectedResponse.Apellido, actualResponse.Apellido);
                Assert.Equal(expectedResponse.Dni, actualResponse.Dni);
                Assert.Equal(expectedResponse.FechaNacimiento, actualResponse.FechaNacimiento);
                Assert.Equal(expectedResponse.Genero, actualResponse.Genero);
                Assert.Equal(expectedResponse.NumContactoEmergencia, actualResponse.NumContactoEmergencia);
                Assert.Equal(expectedResponse.Nacionalidad, actualResponse.Nacionalidad);
                Assert.NotNull(actualResponse.Viaje);
                Assert.Equal(expectedResponse.Viaje.Id, actualResponse.Viaje.Id);
                Assert.Equal(expectedResponse.Viaje.TransporteId, actualResponse.Viaje.TransporteId);
                Assert.Equal(expectedResponse.Viaje.Duracion, actualResponse.Viaje.Duracion);
                Assert.Equal(expectedResponse.Viaje.FechaSalida, actualResponse.Viaje.FechaSalida);
                Assert.Equal(expectedResponse.Viaje.FechaLlegada, actualResponse.Viaje.FechaLlegada);
                Assert.Equal(expectedResponse.Viaje.TipoViaje, actualResponse.Viaje.TipoViaje);
                Assert.Equal(expectedResponse.Viaje.Precio, actualResponse.Viaje.Precio);
            }
        }

        [Fact]

        public void TestUpdatePasajero() 
        {

            //ARRANGE

            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var viajeId = 1;
            var request = new PasajeroRequest
            {
                Nombre = "John",
                Apellido = "Doe",
                Dni = 123456789,
                FechaNacimiento = new DateTime(1990, 10, 15),
                Genero = "Masculino",
                NumContactoEmergencia = 987654321,
                Nacionalidad = "Argentina",
                ViajeId = 2
            };

            var viajeRequest = new Viaje
            {
                ViajeId = 2,
                TransporteId = 3,
                Duracion = "4 horas",
                FechaLlegada = DateTime.Now.AddDays(2),
                FechaSalida = DateTime.Now,
                TipoViaje = "Bus",
                AsientosDisponibles = 40,
                Precio = 1300,
                Pasajeros = new List<Pasajero>()
            };

            var target = new Pasajero
            {
                Nombre = "Sara",
                Apellido = "Smith",
                Dni = 55842548,
                FechaNacimiento = new DateTime(1992, 11, 10),
                Genero = "Femenino",
                NumContactoEmergencia = 1121779949,
                Nacionalidad = "Argentina",
                ViajeId = viajeId,
                Viaje = new Viaje
                {
                    ViajeId = 1,
                    TransporteId = 3,
                    Duracion = "3 horas",
                    FechaLlegada = DateTime.Now.AddDays(2),
                    FechaSalida = DateTime.Now,
                    TipoViaje = "Terrestre",
                    AsientosDisponibles = 50,
                    Precio = 150,
                    Pasajeros = new List<Pasajero>()
                }
            };

            mockPasajeroQuery.Setup(q => q.GetPasajeroById(1)).Returns(target);
            mockViajeQuery.Setup(q => q.GetViajeById(2)).Returns(viajeRequest);
            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ACT

            var result = pasajeroService.UpdatePasajero(1, request);

            //ASSERT
            Assert.Equal(request.Nombre, result.Nombre);
            Assert.Equal(request.Apellido, result.Apellido);
            Assert.Equal(request.Dni, result.Dni);
            Assert.Equal(request.FechaNacimiento, result.FechaNacimiento);
            Assert.Equal(request.Genero, result.Genero);
            Assert.Equal(request.NumContactoEmergencia, result.NumContactoEmergencia);
            Assert.Equal(request.Nacionalidad, result.Nacionalidad);
            Assert.Equal(viajeRequest.Duracion, result.Viaje.Duracion);
            Assert.Equal(viajeRequest.FechaSalida, result.Viaje.FechaSalida);
            Assert.Equal(viajeRequest.FechaLlegada, result.Viaje.FechaLlegada);
            Assert.Equal(viajeRequest.TipoViaje, result.Viaje.TipoViaje);
            Assert.Equal(viajeRequest.Precio, result.Viaje.Precio);
            Assert.Equal(viajeRequest.TransporteId, result.Viaje.TransporteId);
            Assert.Equal(viajeRequest.ViajeId, result.Viaje.Id);

    
        }

        [Fact]

        public void TestUpdatePasajero_PasajeroNull()
        {

            //ARRANGE

            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();
            int pasajeroId = 1;
            mockPasajeroQuery.Setup(q => q.GetPasajeroById(pasajeroId)).Returns((Pasajero)null);
            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            var request = new PasajeroRequest
            {
                Nombre = "John",
                Apellido = "Doe",
                Dni = 123456789,
                FechaNacimiento = new DateTime(1990, 10, 15),
                Genero = "Masculino",
                NumContactoEmergencia = 987654321,
                Nacionalidad = "Argentina",
                ViajeId = 2
            };


            //ACT

            var exception = Assert.Throws<ArgumentException>(() => pasajeroService.UpdatePasajero(pasajeroId, request));

            //ASSERT
            exception.Message.Should().Be($"No se encontró el pasajero con el identificador {pasajeroId}.");
        }

        [Fact]
        public void TestUpdatePasajero_ViajeNull()
        {
            //ARRANGE

            var mockViajeQuery = new Mock<IViajeQuery>();
            var mockPasajeroCommand = new Mock<IPasajeroCommand>();
            var mockPasajeroQuery = new Mock<IPasajeroQuery>();

            var request = new PasajeroRequest
            {
                Nombre = "John",
                Apellido = "Doe",
                Dni = 123456789,
                FechaNacimiento = new DateTime(1990, 10, 15),
                Genero = "Masculino",
                NumContactoEmergencia = 987654321,
                Nacionalidad = "Argentina",
                ViajeId = 2
            };


            var target = new Pasajero
            {
                Nombre = "Sara",
                Apellido = "Smith",
                Dni = 55842548,
                FechaNacimiento = new DateTime(1992, 11, 10),
                Genero = "Femenino",
                NumContactoEmergencia = 1121779949,
                Nacionalidad = "Argentina",
                ViajeId = 1,
                Viaje = new Viaje
                {
                    ViajeId = 1,
                    TransporteId = 3,
                    Duracion = "3 horas",
                    FechaLlegada = DateTime.Now.AddDays(2),
                    FechaSalida = DateTime.Now,
                    TipoViaje = "Terrestre",
                    AsientosDisponibles = 50,
                    Precio = 150,
                    Pasajeros = new List<Pasajero>()
                }
            };

            int pasajeroId = 1;
            mockPasajeroQuery.Setup(q => q.GetPasajeroById(pasajeroId)).Returns(target);
            mockViajeQuery.Setup(q => q.GetViajeById(request.ViajeId)).Returns((Viaje)null);
            var pasajeroService = new PasajeroService(mockPasajeroCommand.Object, mockPasajeroQuery.Object, mockViajeQuery.Object);

            //ACT

            var exception = Assert.Throws<Exception>(() => pasajeroService.UpdatePasajero(pasajeroId, request));

            //ASSERT
            exception.Message.Should().Be($"No se encontró el viaje con el identificador {request.ViajeId}.");
        }


    }
}
