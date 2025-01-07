$(document).ready(function () {
    $(".EditarFC").click(function () {
        var Id = $(this).attr("name");
        window.location.href = "/Facturas/facturas/EditarFacturaCompra?Id=" + Id;
    });

    //Sirve para tener seleccionada una opcion de un select  $('#id_country > option[value="3"]').attr('selected', 'selected');
});