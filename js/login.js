'use strict';

sessionStorage.removeItem('AppUser');
var toastr = new Toastr();

$(document).ready(() => {
    // $('#recover').click(() => {
    //     $('#formLogin').css('height', '0');
    //     setTimeout(() => {
    //         $('#formLogin').hide();
    //         $('#formRecover').show();
    //         setTimeout(() => {
    //             $('#formRecover').css('height', '100%');
    //             $('#strUserRecover').focus();
    //         }, 100);
    //     }, 1500);
    // });

    // $('.goBack').click(() => {
    //     $('#formRecover, #formCod, #formResetPassword').css('height', '0');
    //     $('#strUser, #strPassword, #strUserRecover, #strCodRecover, #strPasswordReset, #strConfirmPasswordReset').val('');
    //     sessionStorage.removeItem('OTP');
    //     sessionStorage.removeItem('AppUser');

    //     setTimeout(() => {
    //         $('#formRecover, #formCod, #formResetPassword').hide();
    //         $('#formLogin').show();
    //         setTimeout(() => {
    //             $('#formLogin').css('height', '100%');
    //             $('#strPassword').focus();
    //         }, 100);
    //     }, 1500);
    // });

    $('#formLogin').submit((e) => {
        e.preventDefault();
        fetch("https://ejemploluis.azurewebsites.net/api/Account/Login", {
        method: 'POST',
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            "User": e.target[0].value,
            "Pass": e.target[1].value
            })
        })
        .then(result => result.text())
        .then(response => {
            if(JSON.parse(response).cod == 0){
                sessionStorage.setItem('AppUser', response);
                location.href = '/inicio.html';
            }else{
                toastr.Error(JSON.parse(response).rpta)
            }
        }).catch(error =>{
            toastr.Warning(error)
        });

    });

    // $('#formRecover').submit((e) => {
    //     e.preventDefault();
    //     ExecQuery({ "userU": e.target[0].value }, 'User/RecoverPassword').then(response => {
    //         toastr.Success(response);
    //         sessionStorage.setItem('AppUser', USER);
    //         $('#formRecover').css('height', '0');
    //         setTimeout(() => {
    //             $('#formRecover').hide();
    //             $('#formCod').show();
    //             setTimeout(() => {
    //                 $('#formCod').css('height', '100%');
    //             }, 100);
    //         }, 1500);
    //         $('#strCodRecover').val('');
    //     }).catch(error => {
    //         toastr.Warning(error)
    //     });
    // });

    // $('#formCod').submit((e) => {
    //     e.preventDefault();
    //     ExecQuery({ "UserName": sessionStorage.AppUser, "Cod": e.target[0].value }, 'User/ValidateCod').then(response => {
    //         sessionStorage.setItem('AppUser', response.Rpta);
    //         sessionStorage.setItem('OTP', COD);
    //         $('#formCod').css('height', '0');
    //         setTimeout(() => {
    //             $('#formCod').hide();
    //             $('#formResetPassword').show();
    //             setTimeout(() => {
    //                 $('#formResetPassword').css('height', '100%');
    //             }, 100);
    //         }, 1500);
    //         $('#strPassword, #strConfirmPassword').val('');
    //     }).catch(error => {
    //         toastr.Warning(error)
    //     });
    // });

    // $('#formResetPassword').submit((e) => {
    //     if (e.target[0].value === '') {
    //         toastr.Info('La contraseña no puede ser vacia');
    //         return;
    //     }
    //     e.preventDefault();
    //     ExecQuery({"User_Id": sessionStorage.AppUser, "Password": e.target[0].value, "OTP": sessionStorage.OTP }, 'User/ChangePasswordOTP').then(response => {
    //         sessionStorage.removeItem('OTP');
    //         sessionStorage.removeItem('AppUser');
    //         sessionStorage.setItem('AppUser', JSON.stringify(response));
    //         location.href = '/Home';
    //     }).catch(error => {
    //         toastr.Warning(error)
    //     });
    // });
});

function newId() {

    let Codigo = '';

    for (let i = 0; i < 3; i++) {

        let str1, str2, str3, str4;
        /* Generar numeros random de acuerdo al codigo ASCII y convertirlos */
        str1 = String.fromCharCode(Math.round((Math.random() * (57 - 48)) + 48));
        str2 = String.fromCharCode(Math.round((Math.random() * (90 - 65)) + 65));
        str3 = String.fromCharCode(Math.round((Math.random() * (122 - 97)) + 97));
        str4 = String.fromCharCode(Math.round((Math.random() * (57 - 48)) + 48));

        /* Integrarlo al codigo */
        Codigo += `${str1}${str2}${str3}${str4}`;

        if (i != 2) {
            Codigo += `-`;
        }

    }

    return Codigo;

}

function Toastr() {

    this.Info = (mesagge, title) => {
        newMesagge(mesagge, title, 'toast-info');
    }

    this.Success = (mesagge, title) => {
        newMesagge(mesagge, title, 'toast-success');
    }

    this.Error = (mesagge, title) => {
        newMesagge(mesagge, title, 'toast-error');
    }

    this.Warning = (mesagge, title) => {
        newMesagge(mesagge, title, 'toast-warning');
    }

    function newMesagge(mesagge, title, type, id = newId()) {

        let data = $('#notifications :last-child').html() || '';

        if (!data.includes(mesagge)) {

            title ? title = `<b class="title">${title}</b>` : title = '';

            $('#notifications').append(`<div class="toast ${type}" id="${id}">${title}${mesagge}</div>`);

            $(`#${id}`).click(() => {
                $(`#${id}`).remove();
            })

            setTimeout(() => {
                $(`#${id}`).remove();
            }, 10000);

        }
    }
}