using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Capa2_Aplicacion.ModuloPrincipal.Servicios;
using Capa4_Persistencia.SqlServer.ModuloBase;

public class Cie11ServicioTests
{
    [Fact]
    public async Task BuscarTermino_DeberiaDevolverResultadosDelMock()
    {
        // Arrange (Preparar)
        var mockApi = new Mock<ICie11ApiClient>();
        mockApi.Setup(x => x.BuscarTerminoAsync("colera"))
               .ReturnsAsync("{ \"destinationEntities\": [ { \"theCode\": \"A00\", \"title\": \"Cólera\" } ] }");

        var servicio = new Cie11Servicio(mockApi.Object); //compilación

        // Act (Actuar)
        var resultados = await servicio.BuscarTermino("colera");

        // Assert (Afirmar)
        Assert.Single(resultados);
        Assert.Equal("A00", resultados[0].Codigo);
        Assert.Equal("Cólera", resultados[0].Titulo);
    }
}
