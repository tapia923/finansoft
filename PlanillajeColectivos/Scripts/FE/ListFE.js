$(document).ready(function () {

    

    function GetTablaFE() {

        var columnas = [
            { data: "Id" },
            { data: "Prefijo" },
            { data: "Numero" },
            { data: "Cufe" },
            { data: "Fecha" },
            { data: "Hora" },
            { data: "Cliente"},
            { data: "EstadoEnvioEmail", className: "colCenter", "render": function (data) { return estado(data); } },
            { data: "Id", className: "colCenter", "render": function (data) { return ' <a href="/Facturas/facturas/DescargarFEPdf?id=' + data + '" class="btn btn-success fa fa-download" title="Descargar"></a>&nbsp<a href="#" class="btn btn-warning btn-xs fa fa-envelope-o" title="Enviar"></a>'; } },
            { data: "Id", className: "colCenter", "render": function (data) { return '<button class="btn btn-primary btn-xs fa fa-pencil-square-o btnDebito" name="'+data+'" title="Nota Debito"></button>'; } },
            { data: "Id", className: "colCenter", "render": function (data) { return '<button class="btn btn-primary btn-xs fa fa-pencil-square-o btnCredito" name="'+data+'" title="Nota Credito"></button>'; } },
            
        ];


        function estado(estado) {
            if (estado) {
                return '<span class="circuloVerde"><i class="fa fa-check"></i></span>'
            } else {
                return '<span class="circuloRojo"><i class="fa fa-times"></i></span>'
            }
        }

        var botones = [
            //{
            //    extend: 'collection',
            //    text: 'Exportar A',
            //    autoClose: true,
            //    buttons: [
            //        {
            //            extend: 'excel',
            //            text: "Excel",
            //            exportOptions: {
            //                columns: [1, 2]
            //            }

            //        }
            //    ]

            //}
            //'excel', 
            {

                extend: 'pdf',
                title: "Facturas Electronicas"

            }

        ]; //fin botones

        agregarDataTable("#TablaListFE", columnas, '/Facturas/facturas/GetListFE', botones, false, true, true);
        table.columns(0).visible(false);
        table.columns(3).visible(false);

        function agregarDataTable(TablaListFE, columnas, urlDatos, botones, scroll, buscador, seleccion) {
            var TraduccionDatatable = {
                "sProcessing": "Procesando...", "sLengthMenu": "Mostrar _MENU_ registros", "sZeroRecords": "No se encontraron resultados", "sEmptyTable": "No hay registros", "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros", "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros", "sInfoFiltered": "(filtrado de un total de _MAX_ registros)", "sInfoPostFix": "", "sSearch": "Buscar:", "sUrl": "", "sInfoThousands": ",", "sLoadingRecords": "Cargando...", "select": { "rows": { _: "Has seleccionado %d filas", 0: "", 1: "1 fila seleccionada" } }, "oPaginate": { "sFirst": "<<", "sLast": ">>", "sNext": ">", "sPrevious": "<" }, "oAria": { "sSortAscending": ": Activar para ordenar la columna de manera ascendente", "sSortDescending": ": Activar para ordenar la columna de manera descendente" }
            };
            
            // iris2 = iris[c(1;10, 51:60, 101:110), ]
            table = $(TablaListFE).DataTable({
                destroy: true,
                dom: 'Bfrtip',
                ajax: {
                    type: "POST",
                    url: urlDatos,
                    contentType: 'application/json; charset=utf-8',
                    data: function (data) { return data = JSON.stringify(data); }
                },
                searching: buscador,
                lengthChange: false,
                autoWidth: false,
                scrollX: scroll,
                columns: columnas,
                buttons: botones,
                deferRender: true,
                select: seleccion,
                language: TraduccionDatatable,
                
            });
            //table.buttons().container().appendTo('.col-sm-6:eq(0)');
        }
    };
    GetTablaFE();

    $("#TablaListFE").on('click','button.btnCredito',function () {
        
        var Id = $(this).attr("name");
        window.location.href = "/FacturacionElectronica/NotaCredito/NotaCredito?Id="+Id;
    });

});