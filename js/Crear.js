'use strict';
$(document).ready(() => {

    $('#formCreate').submit((e) => {
        e.preventDefault();
        fetch("https://ejemploluis.azurewebsites.net/api/Appoiment/CreateAppoiment", {
            method: 'POST',
            headers: {
              "Content-Type": "application/json",
              "Authorization" : `bearer ${JSON.parse(sessionStorage.AppUser).token}`
            },
            body: JSON.stringify({
                "nameClient": e.target[0].value,
                "dateStart": e.target[1].value,
                "dateEstimated": e.target[2].value
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
            alert(response.rpta);
        }).catch(error =>{
            alert(error);
        });    
    });
});
