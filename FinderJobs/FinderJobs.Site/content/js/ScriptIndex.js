//$(document).ready(function () {
//    $.ajax({
//        url: "../Home/VagasJson",
//        success: function (data) {
//            var row = "";
//            $.each(data, function (index, item) {
//                row += '<tr> <td> ' +
//                    item.DataCadastro + "</td> <td> " +
//                    item.Empresa + " </td> <td> " +
//                    item.Descricao + " </td> <td>" +
//                    item.Cep + "</td>" + "<td>" +
//                    item.Habilidades + "</td>" + "<td>" + 
//                    '<button class="btn-details" onclick="pesquisaDistancia(' + item.Id + ')">' + '<span class="ui-icon ui-icon-document ui-state-default" /></button> </td>' +
//                '</tr>'
//            });
//            $("#vagas").html(row);
//        },
//        error: function (result) {
//            alert("Error");
//        }
//    });
//});

function pesquisaDistancia(id) {
    var dados = id;
    $.ajax(
        {
            url: "../home/PesquisaDistancia",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            type: "GET",
            data: JSON.stringify(dados),
            success: function (dados) {
                $(dados).each(function (i) {
                    if (dados[i].Nome != null) {
                        document.getElementById("007").innerHTML = dados[i].Nome + " está à " + dados[i].Distancia +
                                                                " desta empresa e possui " + dados[i].Porcentagem + "% das habilidades.";
                        $("#myModal").modal();
                    }
                    else {
                        document.getElementById("007").innerHTML = "nenhum candidato encontrado.";
                        $("#myModal").modal();
                    }
                });
            },
            error: function (error) {
            }
        });
}
