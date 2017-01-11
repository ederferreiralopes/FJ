
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


function cadastrar(IdEmpresa) {

    var erro = 0;

    var empresa = $('#empresa').val();
    if (empresa.length < 3) {
        erro++;
        document.getElementById("1").innerHTML = "obrigatório";
    }
    else {
        document.getElementById("1").innerHTML = "";
    }

    var descricao = $('#descricao').val();
    if (descricao.length < 3) {
        erro++;
        document.getElementById("2").innerHTML = "obrigatório";
    }
    else {
        document.getElementById("2").innerHTML = "";
    }

    var cep = $('#cep').val();
    if (cep.length < 3) {
        erro++;
        document.getElementById("3").innerHTML = "obrigatório";
    }
    else {
        document.getElementById("3").innerHTML = "";
    }

    if (erro === 0) {


        var Empresa = $('#empresa').val();
        var Descricao = $('#descricao').val();
        var Cep = $('#cep').val();
        var Habilidades = "";
        var habilidadesSelect = habilidades.getSelection();

        for (var i = 0; i < habilidadesSelect.length; i++)
        {
            Habilidades += habilidadesSelect[i].name + ',';
        }

        var inputs = $("input[id*='Habilidade']");
        console.log(inputs);
        for (var i = 0; i < inputs.length; i++) {
            console.log(inputs[i].value);
            Habilidades += inputs[i].value + ",";
        }
        console.log(Habilidades);

        var dados =
        {
            'Empresa': Empresa,
            'IdEmpresa': IdEmpresa,
            'Descricao': Descricao,
            'Cep': Cep,
            'Habilidades': Habilidades
        };

        $.ajax({
            url: "../Vagas/CadastrarVagaJson",
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

