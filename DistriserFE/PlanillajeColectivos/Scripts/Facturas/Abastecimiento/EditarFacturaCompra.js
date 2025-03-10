$(document).ready(function () {
    var AuxProveedor = $("#AuxProveedor").val()
    var AuxFormaPago = $("#AuxFormaPago").val()
    $('#provider> option[value="' + AuxProveedor + '"]').attr('selected', 'selected');
    $('#FormaPago> option[value="' + AuxFormaPago + '"]').attr('selected', 'selected');


    $("#btnGuardar").click(function () {
        $("#btnGuardar").prop("disabled", true);
        var total = $("#valorTotal").val();
        var provider = $("#provider").val();
        var facturaFisica = $("#facturaFisica").val();
        var IdFactura = $("#IdFactura").val();
        var IdFormaPago = $("#FormaPago").val();

        var siHayProductos = $('td:nth-child(1)', "#tablaPedido").text();

        if (siHayProductos != "") {
            if (provider != "0") {
                if (facturaFisica != "") {
                    total = total.replace(".", "");
                    total = total.replace(".", "");
                    total = total.replace(",", "");
                    total = total.replace(",", "");
                    Swal.fire({
                        title: 'Terminar Factura?',
                        icon: 'question',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'SI'
                    }).then((result) => {
                        if (result.value) {
                            $.ajax({
                                type: "POST",
                                url: "/Facturas/facturas/TerminarEditarFacturaCompra",
                                datatype: "Json",
                                data: { IdFactura: IdFactura, provider: provider, facturaFisica: facturaFisica, IdFormaPago: IdFormaPago },
                                success: function (data) {
                                    if (data.status) {

                                        Swal.fire({
                                            title: 'Factura editada correctamente!',
                                            icon: 'success',
                                            confirmButtonColor: '#3085d6',
                                            confirmButtonText: 'Ok'

                                        }).then((result) => {
                                            if (result.value) {
                                                window.location.href = "/Facturas/facturas/facturasAbastecimientos";
                                            };
                                        });

                                    } else {
                                        Swal.fire({
                                            icon: 'error',
                                            title: 'Oops...',
                                            text: 'Error al Realizar Factura!'
                                        });
                                        $("#btnGuardar").prop("disabled", false);
                                    }
                                }
                            });
                        } else {
                            $("#btnGuardar").prop("disabled", false);
                        };
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Ingrese Numero de Factura Fisica'
                    })
                    $("#btnGuardar").prop("disabled", false);
                }
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Seleccione un Proveedor'
                })
                $("#btnGuardar").prop("disabled", false);
            }
        } else {
            $("#btnGuardar").prop("disabled", false);
        }

    });

    $("#btnCancelar").click(function () {
        var IdFactura = $("#IdFactura").val();
        Swal.fire({
            title: '¿Desea salir?',
            text: "",
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = "/Facturas/facturas/IrIndexFacturaCompra?IdFactura="+IdFactura;
            }
        })
    });

    $(document).ready(function () {

        renderTabla();

        $('#myTable').DataTable({
            "language": {
                "url": "/Content/Spanish.json"
            }
        });

        $("#tablaProductos").on('click', 'button.classBtnAgregar', function () {
            var IdFactura = $("#IdFactura").val();

            var id = $(this).attr("id");//id del producto
            var valInput = $("#" + id).val();//Cantidad Pedida


            var OpcionDescuento = $("#SD" + id).val();
            var VD = $("#VD" + id).val();
            var NuevoIva = $("#IVA" + id).val();


            var nuevoPrecio = $("input[name=" + id + "]").val();//Cantidad Pedida
            if (nuevoPrecio == "") {
                nuevoPrecio = "0";
            }

            if (valInput > 0 && nuevoPrecio != 0) {
                $.ajax({
                    type: "POST",
                    url: "/Facturas/facturas/AddProductoEditarCompra",
                    datatype: "Json",
                    data: { id: id, cantidad: valInput, nuevoPrecio: nuevoPrecio, OD: OpcionDescuento, VD: VD, IvaId: NuevoIva, IdFactura: IdFactura },
                    success: function (data) {
                        if (data == 1) {
                            renderTabla();
                            Swal.fire({
                                icon: 'success',
                                title: 'Ok',
                                text: 'Producto Agregado!'
                            })
                        }
                    }
                });
            }
        });
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
                    url: "/Facturas/facturas/EliminarProductoEditarFC",
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
        var IdFactura = $("#IdFactura").val();
        $.ajax({
            type: "POST",
            url: "/Facturas/facturas/GetEditandoFacuturaCompra",
            datatype: "Json",
            data: {IdFactura: IdFactura},
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
                    tr += '<td>' + val.subtotal + '</td>';
                    tr += '<td>' + val.descuento + '</td>';
                    tr += '<td>' + val.iva + '</td>';
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

    $(document).ready(function () {
        $(".chosen-select-single").chosen();
        $(".chosen-select-multiple").chosen();    //DESPUES DEL FINAL DEL DOCUMENTO AL PRINCIPIO DE LOS SCRIPTS
    });

    $(".miles").on({
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

    $(".VD").change(function () {
        var valor = $(this).val();
        if (valor != "") {
            var Id = $(this).attr("id");
            Id = Id.substr(2);
            var Opcion = parseInt($("#SD" + Id).val());
            if (Opcion == 2) {
                valor = valor.split('.').join("");
                valor = parseInt(valor);
                if (valor < 0) {
                    $(this).val("0");
                } else if (valor > 100) {
                    $(this).val("100");
                }
            }//fin if opcion
        } else {
            var valor = $(this).val("0");
        }
    });//fin change .VD

    $(".SelectDescuento").change(function () {
        var Id = $(this).attr("id");
        Id = Id.substr(2);
        $("#VD" + Id).val("0");

    });//fin change .VD

    //FORMATEAR UN NUMERO
    var formatNumberMiles = {
        separador: ".", // separador para los miles
        sepDecimal: ',', // separador para los decimales
        formatear: function (num) {
            num += '';
            var splitStr = num.split('.');
            var splitLeft = splitStr[0];
            var splitRight = splitStr.length > 1 ? this.sepDecimal + splitStr[1] : '';
            var regx = /(\d+)(\d{3})/;
            while (regx.test(splitLeft)) {
                splitLeft = splitLeft.replace(regx, '$1' + this.separador + '$2');
            }
            return this.simbol + splitLeft + splitRight;
        },
        new: function (num, simbol) {
            this.simbol = simbol || '';
            return this.formatear(num);
        }
    }
})