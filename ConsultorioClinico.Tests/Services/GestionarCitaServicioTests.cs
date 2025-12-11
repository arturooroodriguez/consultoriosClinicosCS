using System;
using System.Reflection;
using Capa2_Aplicacion.ModuloPrincipal.Servicio;
using Capa3_Dominio.ModuloPrincipal;
using Capa3_Dominio.ModuloPrincipal.Entidad;
using Capa4_Persistencia.SqlServer.ModuloBase;
using Capa4_Persistencia.SqlServer.ModuloPrincipal;
using Moq;
using NUnit.Framework;

namespace ConsultorioClinico.Test
{
    [TestFixture]
    public class GestionarCitaServicioTests
    {
        private Mock<CodigoSQL> mockCodigoSQL;
        private Mock<CitaSQL> mockCitaSQL;
        private Mock<ConsultaSQL> mockConsultaSQL;
        private Mock<AccesoSQLServer> mockAccesoSQL;
        private GestionarCitaServicio servicio;

        [SetUp]
        public void Setup()
        {
            mockAccesoSQL = new Mock<AccesoSQLServer>();
            mockCodigoSQL = new Mock<CodigoSQL>(mockAccesoSQL.Object);
            mockCitaSQL = new Mock<CitaSQL>(mockAccesoSQL.Object);
            mockConsultaSQL = new Mock<ConsultaSQL>(mockAccesoSQL.Object);

            // Inyectar los mocks usando clase derivada
            servicio = new GestionarCitaServicioForTest(
                mockAccesoSQL.Object,
                mockCitaSQL.Object,
                mockConsultaSQL.Object,
                mockCodigoSQL.Object
            );
        }

        [Test]
        public void RegistrarCita_AsignaCodigosYRegistra()
        {
            // Arrange
            var cita = new Cita
            {
                CitaEstado = "P",
                CitaFechaHora = DateTime.Today.AddDays(1)
            };
            var consulta = new Consulta
            {
                Cita = cita
            };

            // Mockear generación de códigos
            mockCodigoSQL.Setup(m => m.GenerarCodigoUnico("CON", "Gestion.Consulta", "consultaCodigo"))
                         .Returns("CON001");
            mockCodigoSQL.Setup(m => m.GenerarCodigoUnico("CIT", "Gestion.cita", "citaCodigo"))
                         .Returns("CIT001");

            // Act
            servicio.RegistrarCita(consulta);

            // Assert
            Assert.That(cita.CitaCodigo, Is.EqualTo("CIT001"));
            Assert.That(consulta.ConsultaCodigo, Is.EqualTo("CON001"));

            mockCitaSQL.Verify(m => m.CrearCita(cita), Times.Once);
            mockConsultaSQL.Verify(m => m.CrearConsulta(consulta), Times.Once);
            mockAccesoSQL.Verify(m => m.IniciarTransaccion(), Times.Once);
            mockAccesoSQL.Verify(m => m.TerminarTransaccion(), Times.Once);
        }



        // Clase derivada para inyectar los mocks
        private class GestionarCitaServicioForTest : GestionarCitaServicio
        {
            public GestionarCitaServicioForTest(
                AccesoSQLServer acceso,
                CitaSQL citaSQL,
                ConsultaSQL consultaSQL,
                CodigoSQL codigoSQL)
            {
                AssignPrivateField("citaSQL", citaSQL);
                AssignPrivateField("consultaSQL", consultaSQL);
                AssignPrivateField("codigoSQL", codigoSQL);
                AssignPrivateField("accesoSQLServer", acceso);
            }

            private void AssignPrivateField(string fieldName, object value)
            {
                var baseType = typeof(GestionarCitaServicio);
                var field = baseType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                    throw new InvalidOperationException($"No se encontró el campo '{fieldName}' en '{baseType.FullName}'");
                field.SetValue(this, value);
            }
        }
    }
}
