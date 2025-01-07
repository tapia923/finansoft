$(document).ready(function () {

    cookie = {
        set: function (name, value) {
            document.cookie = name + "=" + value;
        },
        get: function (name) {
            cookies = document.cookie;
            r = cookies.split(';').reduce(function (acc, item) {
                let c = item.split('='); //'nome=Marcelo' transform in Array[0] = 'nome', Array[1] = 'Marcelo'
                c[0] = c[0].replace(' ', ''); //remove white space from key cookie
                acc[c[0]] = c[1]; //acc == accumulator, he accomulates all data, on ends, return to r variable
                return acc; //here do not return to r variable, here return to accumulator
            }, []);
        }
    };

    //funciones para gastos
    $("#agregar").click(function () {
        funcAgregar();
    });

    function funcAgregar() {
        var newTR = $('<tr>').appendTo("#anotaciones");
        
        newTR.append('<td><input type="text" class="form-control descripcion"></td>');
        newTR.append('<td><input type="text" class="form-control valorFilas"></td>');
        newTR.append('<td><button class="borrar btn btn-danger btn-xs borrarFilas">X</button></td>');
    }

    $(document).on('click', '.borrarFilas', function () {
        $(this).parents('tr').remove();
    });

    $('#anotaciones').on('mouseenter', '.valorFilas', function () {
        $(this).on({
            "focus": function (event) {
                $(event.target).select();
            },
            "keyup": function (event) {
                $(event.target).val(function (index, value) {
                    return value.replace(/\D/g, "")
                        .replace(/([0-9])([0-9]{3})$/, '$1.$2')
                        .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ".");
                });
            }
        });
    });

    //fin fucniones para gastos

    //funciones para otros
    $("#agregarOtro").click(function () {
        funcAgregarOtro();
    });

    function funcAgregarOtro() {
        var newTR = $('<tr>').appendTo("#anotacionesOtro");

        newTR.append('<td><input type="text" class="form-control descripcionOtro"></td>');
        newTR.append('<td><input type="text" class="form-control valorFilasOtro"></td>');
        newTR.append('<td><button class="borrar btn btn-danger btn-xs borrarFilasOtro">X</button></td>');
    }

    $(document).on('click', '.borrarFilasOtro', function () {
        $(this).parents('tr').remove();
    });

    $('#anotacionesOtro').on('mouseenter', '.valorFilasOtro', function () {
        $(this).on({
            "focus": function (event) {
                $(event.target).select();
            },
            "keyup": function (event) {
                $(event.target).val(function (index, value) {
                    return value.replace(/\D/g, "")
                        .replace(/([0-9])([0-9]{3})$/, '$1.$2')
                        .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ".");
                });
            }
        });
    });
    //fin funciones para otros


    $("#btnReporte").click(function () {
        var gastos = new Array();
        var otros = new Array();

        $('.valorFilas').each(function () {
            var gasto = new Array();
            gasto.push($(this).parents('tr')["0"].cells["0"].children["0"].value);
            gasto.push($(this).parents('tr')["0"].cells[1].children["0"].value);
            gastos.push(gasto);

        });

        $('.valorFilasOtro').each(function () {
            var otro = new Array();
            otro.push($(this).parents('tr')["0"].cells["0"].children["0"].value);
            otro.push($(this).parents('tr')["0"].cells[1].children["0"].value);
            otros.push(otro);
        });

        cookie.set('gastos', gastos);
        cookie.set('otros', otros);

        $.ajax({
            type: "POST",
            url: "/Informes/Informes/informeDiario",
            datatype: "Json",
            data: {
                gastos: gastos,
                otros: otros
            },//solo para enviar datos
        });

        //$.ajax({
        //    type: "POST",
        //    url: "/Informes/Informes/informeDiario",
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    data: {
        //        gastos: gastos,
        //        otros: otros
        //    },            
        //});
    });


    
});


//var punto = entrada.split('.').join("");