
function validar()
{
    var erro = 0;

    var habilidades = document.getElementById("Habilidade").value;
    if (habilidades.length < 1) {
        erro++;
        document.getElementById("1").innerHTML = "obrigatório";
    }
    else if (habilidades.length > 25) {
        erro++;
        document.getElementById("1").innerHTML = "limite de 25 caracteres excedido";
    }

    else {
        document.getElementById("1").innerHTML = "";
    }

    if (erro === 0) {
        document.forms["cadastro"].submit();
    }
}

