using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capa4_Persistencia.SqlServer.ModuloBase;
using Newtonsoft.Json;

namespace Capa2_Aplicacion.ModuloPrincipal.Servicios
{
    public class Cie11Servicio
    {
        private readonly ICie11ApiClient _cie11ApiClient;

        //  Nuevo constructor que recibe la interfaz (inyectable)
        public Cie11Servicio(ICie11ApiClient apiClient)
        {
            _cie11ApiClient = apiClient;
        }
        // Constructor para uso normal en el sistema
        public Cie11Servicio() : this(new Cie11ApiClient())
        {
        }

        public async Task<List<Cie11Resultado>> BuscarTermino(string termino)
        {
            var jsonResponse = await _cie11ApiClient.BuscarTerminoAsync(termino);
            var resultado = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            var listaResultados = new List<Cie11Resultado>();
            foreach (var entity in resultado.destinationEntities)
            {
                listaResultados.Add(new Cie11Resultado
                {
                    Codigo = entity.theCode,
                    Titulo = entity.title //CORRECCION
                });
            }

            return listaResultados;
        }
    }

    public class Cie11Resultado
    {
        public string Codigo { get; set; }
        public string Titulo { get; set; }
    }
}
