using System.Threading.Tasks;

namespace Capa4_Persistencia.SqlServer.ModuloBase
{
    public interface ICie11ApiClient
    {
        Task<string> BuscarTerminoAsync(string termino);
    }
}
