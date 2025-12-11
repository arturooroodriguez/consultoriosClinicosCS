var table; // Variable global para la instancia de DataTable

$(document).ready(function () {
    inicializarTablaCitas();
    agregarEventosFiltros();

});

function inicializarTablaCitas() {
    var urlListarCitas = $('#tabla_Citas').data("urlListarCitas");

    if (!urlListarCitas) return;

    table = $('#tabla_Citas').DataTable({
        "dom": "lrtip",
        "ajax": {
            "url": urlListarCitas,
            "type": "GET",
            "dataSrc": function (json) {
                if (json?.consultaExitosa) {
                    return json.data;
                } else {
                    alert(json?.mensaje || "Error en la consulta");
                    return [];
                }
            }
        },
        "columns": obtenerColumnas(),
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json"
        },
        "responsive": false,
        "ordering": false,
    });
}

function obtenerColumnas() {
    return [
        { "data": "CitaCodigo" },
        { "data": "NombrePaciente" },
        { "data": "MedicoNombre" },
        { "data": "Especialidad" },
        { "data": "Fecha" },
        { "data": "Estado" },
        {
            data: null,
            render: function (data, type, row) {
                return `
                    <button class="btn btn-info" onclick="verCita('${row.CitaCodigo}')">Ver</button>
                    <button class="btn btn-danger" onclick="cancelarCita('${row.CitaCodigo}')">Cancelar</button>
                `;
            }
        },

        // Columnas adicionales (ocultas) para filtrar
        { "data": "EspecialidadCod", "visible": false }, // 7
        { "data": "MedicoCodigo", "visible": false },    // 8
        {
            "data": "FechaFilter",
            "visible": false,
            "render": function (data, type, row) {
                const fecha = row.Fecha.split(' ')[0];
                return fecha;
            }
        },
    ];
}

function agregarEventosFiltros() {
    // Filtro por Médico
    $('#inputSelectMedico').on('change', function () {
        if ($.fn.dataTable.isDataTable('#tabla_Citas')) {
            table.column(8).search(this.value).draw();
        }
    });

    // Filtro por Estado
    $('#inputSelectEstado').on('change', function () {
        if ($.fn.dataTable.isDataTable('#tabla_Citas')) {
            table.column(5).search(this.value).draw();
        }
    });

    // Filtro por Especialidad
    $('#inputSelectEspecialidad').on('change', function () {
        if ($.fn.dataTable.isDataTable('#tabla_Citas')) {
            table.column(7).search(this.value).draw();
        }
    });

    // Filtro por Fecha
    $('#inputFecha').on('change', function () {
        if ($.fn.dataTable.isDataTable('#tabla_Citas')) {
            let selectedDate = this.value;  // yyyy-mm-dd
            let formattedDate = selectedDate.split('-').reverse().join('-'); // dd-mm-yyyy
            table.column(9).search(formattedDate).draw();
        }
    });

    // Resetear filtros
    $('#resetFilter').on('click', function () {
        $('#inputSelectEspecialidad').val('');
        $('#inputSelectMedico').val('');
        $('#inputSelectEstado').val('');
        $('#inputFecha').val('');

        if ($.fn.dataTable.isDataTable('#tabla_Citas')) {
            table.search('').columns().search('').draw();
        }
    });
}
