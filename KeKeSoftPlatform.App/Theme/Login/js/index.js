/*
---------------------------
-Click on the Button Join !-
---------------------------
*/


var t = 1;

function join_1() {
        document.querySelector('.cont_form_join').style.bottom = '-420px';
        setTimeout(function () {
            document.querySelector('.cont_join').className = 'cont_join cont_join_form_act cont_join_finish';
        }
           , 500);
}