using Capa3_Dominio.ModuloPrincipal.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa3_Dominio.ModuloPrincipal.TransferenciaDatos
{
    public class ConsultaCompletaDTO
    {
        public Consulta Consulta { get; set; }
        public List<DetallesConsulta> DetallesConsulta { get; set; }
        public List<Diagnostico> Diagnosticos { get; set; }
        public List<RecetaMedica> Recetas { get; set; }
    }
}
