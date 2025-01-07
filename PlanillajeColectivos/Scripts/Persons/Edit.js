$(document).ready(function () {

    $("#cupocredito").val("0")

    document.getElementById("departamento").disabled = true;
    document.getElementById("muniAux").disabled = true;

    $("#pais").change(function () {

        var pais = $(this).val();

        if (pais != "") {
            $("#departamento").empty();
            $("#muniAux").empty();


            $.ajax({
                url: '/Geo/Geo/GetDepartamento',
                datatype: "Json",
                data: {
                    id: pais
                },
                type: 'post',
            }).done(function (data) {
                if (data.status) {
                    document.getElementById("departamento").disabled = false;

                    var select = document.getElementById("departamento");
                    var option = document.createElement("option");
                    option.value = "";
                    option.text = "--Seleccione--";
                    select.appendChild(option);
                    $.each(data.departamento, function (key, val) {
                        var option = document.createElement("option");
                        option.text = val[0];
                        option.value = val[1];
                        select.appendChild(option);
                    });//fin foreach

                }
            });//fin ajax
        } else {
            $("#departamento").empty();
            $("#muniAux").empty();
            document.getElementById("departamento").disabled = true;
            document.getElementById("muniAux").disabled = true;
        }//fin else

    });


    $("#departamento").change(function () {

        var departamento = $(this).val();


        if (departamento != "") {
            $("#muniAux").empty();


            $.ajax({
                url: '/Geo/Geo/GetMunicipio',
                datatype: "Json",
                data: {
                    id: departamento
                },
                type: 'post',
            }).done(function (data) {
                if (data.status) {
                    document.getElementById("muniAux").disabled = false;

                    var select = document.getElementById("muniAux");
                    var option = document.createElement("option");
                    option.value = "";
                    option.text = "--Seleccione--";
                    select.appendChild(option);
                    $.each(data.municipio, function (key, val) {
                        var option = document.createElement("option");
                        option.text = val[0];
                        option.value = val[1];
                        select.appendChild(option);
                    });//fin foreach

                }
            });//fin ajax
        } else {
            $("#muniAux").empty();
            document.getElementById("muniAux").disabled = true;
        }//fin else

    });//fin change departamento

    $("#muniAux").change(function () {

        var muni = $(this).val();
        $("#municipio").val(muni);

    });

});