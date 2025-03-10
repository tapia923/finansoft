$(document).ready(function () {
    $(".chosen-select-single").chosen();
    $(".chosen-select-multiple").chosen();

    renderTabla();

    $('#myTable').DataTable({
        "language": {
            "url": "/Content/Spanish.json"
        }
    });

    $(".classBtnAgregar").click(function () {
        var id = $(this).attr("id");
        var valInput = $("#" + id).val();
        var precioSelect = $(".precio-" + id).val();
        var precioOtro = $(".precioOtro-" + id).val();
        var IdFacturaFE = $("#InputId").val();

        if (precioOtro != "") {
            precioOtro = precioOtro.replace(".", "");
            precioOtro = precioOtro.replace(".", "");
            precioOtro = precioOtro.replace(".", "");
            precioOtro = precioOtro.replace(",", "");
            precioOtro = precioOtro.replace(",", "");
            precioOtro = precioOtro.replace(",", "");
        } else {
            precioOtro = 0;
        }
        if (valInput > 0) {
            $.ajax({
                type: "POST",
                url: "/Procesos/Procesos/AddProductoDevolviendoNC",
                datatype: "Json",
                data: { id: id, cantidad: valInput, precioSelect: precioSelect, precioOtro: precioOtro, IdFacturaFE: IdFacturaFE },
                success: function (data) {
                    if (data == 1) {
                        renderTabla();
                        //Swal.fire({
                        //    icon: 'success',
                        //    title: 'Ok',
                        //    text: 'Producto Agregado!'
                        //})
                    }
                    $("#elBuscador").val("");
                    $("#elBuscador").focus();
                }
            });
        }
    });

    $("#tablaPedido").on('click', 'button.borrar', function () {
        var id = $(this).parents('tr');
        var cId = $('td:nth-child(1)', id).text();
        

        Swal.fire({
            title: 'Quitar Producto?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'SI'
        }).then((result) => {
            if (result.value) {

                $.ajax({
                    type: "POST",
                    url: "/Procesos/Procesos/DeleteProductoContado",
                    datatype: "Json",
                    data: { id: cId },//solo para enviar datos
                    success: function (data) {
                        Swal.fire(
                            'Eliminado!',
                            '',
                            'success'
                        )
                        renderTabla();
                    }
                });
            };
        });
    });


    function renderTabla() {
        $("#tablaPedido").empty();
        var IdFacturaFE = $("#InputId").val();
        $.ajax({
            type: "POST",
            url: "/Procesos/Procesos/getDevolviendoNC",
            datatype: "Json",
            data: {IdFacturaFE: IdFacturaFE},
            success: function (data) {
                data = JSON.parse(data);
                var total = 0;
                $.each(data, function (key, val) {

                    total = total + val.total;

                    var tr = '<tr>';
                    tr += '<td>' + val.operatioId + '</td>';
                    tr += '<td>' + (key + 1) + '</td>';
                    tr += '<td>' + val.cantidad + '</td>';
                    tr += '<td>' + val.codigoBarras + '</td>';
                    tr += '<td>' + val.nombre + '</td>';
                    tr += '<td>' + val.unidad + '</td>';
                    tr += '<td>' + val.total + '</td>';
                    tr += '<td><button class="borrar btn btn-danger btn-xs" id="' + val.operatioId + '">X</button></td>';
                    tr += '</tr>';
                    $('#tablaPedido').append(tr);
                });
                total = parseInt(total);
                $("#valorTotal").val(formatNumberMiles.new(total));
            }
        });
    }

    $("#btnGuardar").click(function () {
        Swal.fire({
            title: '',
            text: "¿Realizar nota crédito?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.value) {
                var siHayProductos = $('td:nth-child(1)', "#tablaPedido").text();
                if (siHayProductos != "") {
                    RealizarDevolucion();
                } else {
                    Swal.fire({
                        icon: 'warning',
                        title: 'No has realizado cambios en tu factura',
                        text: '',
                    })
                } 
            }//fin if result.value 
        })
    });

    $("#btnCancelar").click(function () {

    });

    function RealizarDevolucion() {

        var IdFactura = $("#InputId").val();
        var Motivo = $("#motivoNC").val();
        var RazonMotivo = $("#RazonMotivoNC").val();
        
        $.ajax({
            url: '/FacturacionElectronica/NotaCredito/RealizarNC',
            datatype: "Json",
            data: { IdFacturaFE: IdFactura, Motivo: Motivo, RazonMotivo: RazonMotivo  },
            type: 'post',
        }).done(function (data) {
            if (data.status === true) {
                alert("aceptado");
            }
            else if (data.status === false) {
                alert("Ha ocurrido un error");
            }
        });
    };


})