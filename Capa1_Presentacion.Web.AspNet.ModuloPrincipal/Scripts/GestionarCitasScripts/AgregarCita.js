// AgregarCita.js

$(document).ready(function () {
    inicializarEventosCita();
});

function inicializarEventosCita() {
    $('#btnGuardarCita').on('click', function (e) {
        e.preventDefault();
        guardarCita();
    });

    $('#ModalNuevaCita').on('show.bs.modal', function () {
        reiniciarFormularioCita();
    });
}

function guardarCita() {
    
    var citaData = obtenerDatosFormularioCita();
    if (!validarDatosCita(citaData)) {
        alert('Por favor, Complete los datos Daniel');
        return;
    }
    $.ajax({
        url: urlRegistrarCita, 
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(citaData),
        success: function (response) {
            if (response.transaccionExitosa) {
                alert(response.mensaje);
                $('#ModalNuevaCita').modal('hide');
                cargarHorarios();

            } else {
                alert('Error al registrar la cita: ' + response.mensaje);
            }
        },
        error: function (xhr, status, error) {
            alert('Error al realizar la solicitud: ' + error);
        }
    });
}

//aqui obtengo el valor de cada input
function obtenerDatosFormularioCita() {
    console.log('Obteniendo datos del formulario de cita...');
    return {
        CitaFechaHora: $('#inputFechaHora').val(),
        PacienteCodigo: $('#pacienteCodigo').text(),
        MedicoCodigo: $('#inputMedicoCodigo').val(),
        TipoConsultaCodigo: $('#inputSelectTipoConsulta').val()
    };

}

function validarDatosCita(citaData) {
    return citaData.CitaFechaHora &&
           citaData.PacienteCodigo &&
           citaData.MedicoCodigo &&
           citaData.TipoConsultaCodigo;
}

function reiniciarFormularioCita() {
    $('#formCita')[0].reset(); 
    $('#datosPaciente').collapse('hide');
    limpiarSelectMedico(); 
}
