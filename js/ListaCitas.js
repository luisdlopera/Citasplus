'use strict';
$(document).ready(() => {
    fetch("https://ejemploluis.azurewebsites.net/api/Appoiment/GetAppoimentByUser", {
        method: 'GET',
        headers: {
            "Content-Type": "application/json",
            "Authorization": `bearer ${JSON.parse(sessionStorage.AppUser).token}`
        }
    })
    .then(result => result.json())
    .then(response => {
        console.log(response);
        let html = ``;
        response.appoiments.forEach(function (elemento) {
            html += `<tr>
                    <td>${elemento.dtDateStart.replace("T", "  ")}</td>
                    <td>${elemento.intEnd} minutos</td>
                    <td>${elemento.strNameClient}</td>
                    <td><button class="delete-button" data-id="${elemento.id}">Eliminar</button></td>
                </tr>`;
        });
        $("#tableAppoiment").append(html);

        // Asignar controlador de eventos a los botones de eliminación
        $(".delete-button").click(function () {
            const id = $(this).data("id");
            EliminarCita(id, $(this));
        });
    })
    .catch(error => alert(error));
});

function EliminarCita(id, $button) {
    fetch("https://ejemploluis.azurewebsites.net/api/Appoiment/DeleteAppoiment", {
        method: 'DELETE',
        headers: {
            "Content-Type": "application/json",
            "Authorization": `bearer ${JSON.parse(sessionStorage.AppUser).token}`
        },
        body: JSON.stringify({
            "Appoiment_Id": `${id}`
        })
    })
    .then(result => {
        if (result.status === 401) {
            // Acción para manejar la respuesta 401, por ejemplo, redirigir al usuario a la página de inicio de sesión
            location.href = '/login.html';
        }
        return result.text();
    })
    .then(response => {
        alert(JSON.parse(response).rpta);
        // Si la eliminación fue exitosa, ocultar la fila correspondiente
        if (JSON.parse(response).cod == 0) {
            $button.closest("tr").remove();
        }
    })
    .catch(error => alert(error));
}
