'use strict';
if (!sessionStorage.AppUser) {
    location.href = '/401';
} else {
    fetch("https://ejemploluis.azurewebsites.net/api/Account/ValidateUser", {
        method: 'POST',
        headers: {
          "Content-Type": "application/json",
          "Authorization" : `bearer ${JSON.parse(sessionStorage.AppUser).token}`
        }
      })
      .then(result => {
        if (result.status === 401) {
            // Acción para manejar la respuesta 401, por ejemplo, redirigir al usuario a la página de inicio de sesión
            location.href = '/login.html';
        }
        return result.text();
    })
    .then(response => {
        sessionStorage.setItem('AppUser', response);
    }).catch(error =>{
        alert(error);
        location.href = '/401';
        })
}
$(document).ready(() => {

    $('#LogOut').click(() => {
        location.href = '../../';
    });

});