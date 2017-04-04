//preenche dropdown de habilidades
var habilidades = '';
$(document).ready(function () {
    $.ajax({
        url: "../Home/HabilidadesJson",
        success: function (data) {
            habilidades = $('#magicsuggest').magicSuggest({
                maxSelection: 15, allowFreeEntries: false,
                data: data
            });
        },
        error: function (result) {
            alert("Error");
        }
    });
});

function validar()
{
    var erro = 0;
    var nome = document.getElementById("nome").value;
    if (nome.length < 3)
    {
        erro++;
        document.getElementById("1").innerHTML = "obrigatório";
    }
    else {
        document.getElementById("1").innerHTML = "";
    }
    
    var cep = document.getElementById("cep").value;
    if (cep.length < 3) {
        erro++;
        document.getElementById("2").innerHTML = "obrigatório";
    }
    else {
        document.getElementById("2").innerHTML = "";
    }

    if (erro === 0)
    {
        var Nome = $('#nome').val();
        var Cep = $('#cep').val();
        var Habilidades = "";
        var habilidadesSelect = habilidades.getSelection();

        for (var i = 0; i < habilidadesSelect.length; i++) {
            Habilidades += habilidadesSelect[i].name + ',';
        }

        var dados =
        {
            'Nome': Nome,
            'Cep': Cep,
            'Habilidades': Habilidades
        };

        $.ajax({
            url: "../Home/CadastrarCandidato",
            type: "post",
            data: JSON.stringify(dados),
            contentType: 'application/json',
            dataType: 'json',
            success: function (data) {
                document.getElementById("mensagem").innerHTML = data.mensagem;
            },
            error: function (result) {
                alert("Error");
            }
        });
    }
}

