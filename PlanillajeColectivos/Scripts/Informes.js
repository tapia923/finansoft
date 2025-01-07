$(document).ready(function () {
    $('#librosaux').on('show.bs.modal', function (e) {
        $("#count").hide();
        $("#table2").hide();
        $("#table1").hide();
        $("#level").hide();
        $("#niveles2").hide();
        $("#chkTerceros").hide();
        $("#costos").hide();
        $("#table3").hide();
        var informe = $(e.relatedTarget).data('informe');

        
            var modal = $(this); 
        
        
        modal.find("#informe").val(informe);
        

        if (informe == 1) {
            $("#table2").show();
            $("#niveles2").show();
            $("#chkTerceros").show();
            $("#costos").show();
            $("#title").html("BALANCE DE COMPROBACION");
        } else if (informe == 2) {
            $("#table1").show();
            $("#title").html(" ESTADO DE RESULTADOS");
        } else if (informe == 3) {
            $("#count").show();
            $("#table1").show();
            $("#title").html(" BALANCE GENERAL");
        } else if (informe == 4) {
            $("#table2").show();
            $("#title").html("FACTURACIÓN");
        } else if (informe == 5) {
            $("#table2").show();
            $("#title").html("ABASTECIMIENTOS");
        } else if (informe == 6) {
            $("#table1").show();
            $("#level").show();
            $("#title").html("LIBRO AUXILIAR");
        } else if (informe == 8) {
            $("#count").show();
            $("#table2").show();
            $("#title").html("AUXILIAR POR CUENTA");
        } else if (informe == 9) {
            $("#table3").show();
            $("#title").html("COMPROBANTE INFORME DIARIO");
        }
        
    });


    $("#nivel2").change(function () {
        var nivel = $(this).val();
        if (nivel == "5") {
            $("#chkTerceros").show();
        } else {
            $("#chkTerceros").hide();
        }
    });
});